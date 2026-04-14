package itmcom

import (
	"time"
)

type ATSWPParseStats struct {
	ParsedFrames int
	Malformed    int
}

// The legacy protocol parsing is reused for ATSWP: GetBatchsATSWP extracts a stuffed/encoded
// ATSWP frame and then feeds the decoded content (without the last byte) into the legacy parser.
//
// This is a pragmatic Go translation of ITMCOMDLL.GetBatchsATSWP with command handling ignored for MVP.
func parseATSWPFrames(
	receive []byte,
	to SendTo,
	modemID string,
	modemType ModemType,
	legacyRemain []byte,
	atswpRemain []byte,
	onFrame func(frame BatchRecord),
) (frames []BatchRecord, newLegacyRemain []byte, newATSWPRemain []byte) {
	frames, newLegacyRemain, newATSWPRemain, _ = parseATSWPFramesDetailed(receive, to, modemID, modemType, legacyRemain, atswpRemain, onFrame)
	return frames, newLegacyRemain, newATSWPRemain
}

func parseATSWPFramesDetailed(
	receive []byte,
	to SendTo,
	modemID string,
	modemType ModemType,
	legacyRemain []byte,
	atswpRemain []byte,
	onFrame func(frame BatchRecord),
) (frames []BatchRecord, newLegacyRemain []byte, newATSWPRemain []byte, stats ATSWPParseStats) {
	// RefreshAllBytes equivalent for the ATSWP -> legacy feeding.
	// We keep legacyRemain separately; at the beginning of this function "receive" should already
	// include aswpRemain prefix. But in the original algorithm remainATSWP is passed separately.
	// Here we follow the same effect: legacyRemain starts as provided.
	var batchsAtswp = len(receive)
	index1 := 0

	sizeTotal := len(receive)
	_ = sizeTotal

	for batchsAtswp >= 7 && len(receive) >= 7 {
		if index1 != len(receive) {
			if receive[index1] != byte(192) {
				index1++
				batchsAtswp = len(receive) - index1
				continue
			}

			num1 := 6
			// batchType is receive[index1+1]
			var batchType byte
			if index1+1 >= len(receive) {
				break
			}
			batchType = receive[index1+1]
			// If Config batchType == 197 => num1 = 4
			if batchType == 197 {
				num1 = 4
			}

			index1 += num1

			num2 := 0
			foundEnd := false
			byteList := make([]byte, 0, 256)
			for index1+num2 < len(receive) {
				num3 := receive[index1+num2]
				num2++
				byteList = append(byteList, num3)
				if num3 == byte(193) {
					foundEnd = true
					break
				}
			}
			if !foundEnd {
				// Incomplete chunk; keep remain for next read window.
				break
			}

			// Restore prefix bytes (num1 bytes right before index1) to the beginning of the batch.
			prefixStart := index1 - num1
			if prefixStart < 0 {
				stats.Malformed++
				break
			}
			restored := make([]byte, 0, num1+len(byteList))
			restored = append(restored, receive[prefixStart:prefixStart+num1]...)
			restored = append(restored, byteList...)

			// Clear byte stuffing (195 escape).
			unstuffed, okUnstuff := clearByteStaffingDetailed(restored)
			if !okUnstuff {
				stats.Malformed++
				index1 += num2
				batchsAtswp = len(receive) - index1
				continue
			}
			if len(unstuffed) < 4 || unstuffed[0] != 192 || unstuffed[len(unstuffed)-1] != 193 {
				stats.Malformed++
				index1 += num2
				batchsAtswp = len(receive) - index1
				continue
			}
			if batchType <= 5 {
				if len(unstuffed) < 6 {
					stats.Malformed++
					index1 += num2
					batchsAtswp = len(receive) - index1
					continue
				}
				declared := int(unstuffed[4])<<8 | int(unstuffed[5])
				if declared != len(unstuffed) {
					stats.Malformed++
					index1 += num2
					batchsAtswp = len(receive) - index1
					continue
				}
			} else {
				declared := int(unstuffed[2])<<8 | int(unstuffed[3])
				if declared != len(unstuffed) {
					stats.Malformed++
					index1 += num2
					batchsAtswp = len(receive) - index1
					continue
				}
			}

			// Command handling: C# stores into command queues. MVP: ignore.
			isCommand := unstuffed != nil && (batchType > 5)
			if isCommand {
				onFrame(BatchRecord{
					Timestamp:  time.Now(),
					ModemID:    modemID,
					ModemType:  modemType,
					SiteIDFrom: -2, // command source marker
					SiteIDTo:   -3, // command answer marker
					LinkID:     batchType,
					BatchHex:   hexByteString(unstuffed),
					Raw:        bytesCopy(unstuffed),
				})
				stats.ParsedFrames++
			} else {
				// RefreshAllBytes(remainSize, remain, extraSize=numArray2.Length-1, extra=numArray2)
				extraSize := len(unstuffed) - 1
				if extraSize < 0 {
					extraSize = 0
				}
				allSize := len(legacyRemain) + extraSize
				if allSize > 0 {
					receive1 := make([]byte, allSize)
					copy(receive1, legacyRemain)
					if extraSize > 0 {
						copy(receive1[len(legacyRemain):], unstuffed[:extraSize])
					}

					// Feed into legacy parser; legacy parser itself handles frame splitting.
					var out []BatchRecord
					outRemain := parseLegacyToBatches(receive1, to, modemID, modemType, unstuffed, func(b BatchRecord) {
						out = append(out, b)
						stats.ParsedFrames++
					})
					legacyRemain = outRemain
					frames = append(frames, out...)
				}
			}

			index1 += num2
			batchsAtswp = len(receive) - index1
		} else {
			break
		}
	}

	if batchsAtswp > 0 && batchsAtswp < 10000 {
		newATSWPRemain = make([]byte, batchsAtswp)
		copy(newATSWPRemain, receive[index1:index1+batchsAtswp])
	}

	newLegacyRemain = legacyRemain
	return frames, newLegacyRemain, newATSWPRemain, stats
}

// clearByteStaffingDetailed matches ITMCOMDLL.ClearByteStaffing and additionally
// reports malformed escape tails.
func clearByteStaffingDetailed(oldBatch []byte) ([]byte, bool) {
	if len(oldBatch) == 0 {
		return nil, true
	}
	out := make([]byte, 0, len(oldBatch))
	for i := 0; i < len(oldBatch); i++ {
		num1 := oldBatch[i]
		if num1 == byte(195) {
			i++
			if i >= len(oldBatch) {
				return out, false
			}
			num2 := oldBatch[i]
			out = append(out, byte(uint8(num2)&223))
			continue
		}
		out = append(out, num1)
	}
	return out, true
}

// parseLegacyToBatches wraps parseLegacyFrames and adapts callback style.
// We ignore the legacy "onFrame" timestamp differences and set them consistently.
func parseLegacyToBatches(
	receive []byte,
	to SendTo,
	modemID string,
	modemType ModemType,
	_ []byte,
	onBatch func(BatchRecord),
) (remain []byte) {
	// parseLegacyFrames already sets timestamp and Raw/hex.
	return parseLegacyFrames(receive, to, modemID, modemType, func(b BatchRecord) {
		onBatch(b)
	})
}

// ensure time is used to keep linter happy; parseLegacyFrames uses time.Now.
var _ = time.Now
