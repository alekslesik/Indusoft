package itmcom

import "time"

type LegacyParseStats struct {
	ParsedFrames int
	Malformed    int
}

// parseLegacyFrames is a direct Go translation of ITMCOMDLL.GetBatchs (legacy/ModemType.Legacy)
// with the side effect replaced by callback onFrame.
//
// It parses a stream of raw bytes into frames terminated by byte 13,
// where the header is an escaped/unescaped 9-byte DataConvert block.
func parseLegacyFrames(
	receive []byte,
	to SendTo,
	modemID string,
	modemType ModemType,
	onFrame func(frame BatchRecord),
) (remain []byte) {
	remain, _ = parseLegacyFramesDetailed(receive, to, modemID, modemType, onFrame)
	return remain
}

func parseLegacyFramesDetailed(
	receive []byte,
	to SendTo,
	modemID string,
	modemType ModemType,
	onFrame func(frame BatchRecord),
) (remain []byte, stats LegacyParseStats) {
	sizeTotal := len(receive)
	batchs := len(receive)
	begin := 0
	flag1 := false

	for batchs >= 9 && len(receive) >= 9 {
		var numArray [9]byte
		num1 := 0
		flag2 := false
		flag3 := true

		for index := 0; index < 9; index++ {
			if num1 >= batchs {
				flag2 = true
				break
			}
			if !flag3 && receive[begin+num1] == byte(10) {
				// Corrupted stream: shift begin forward.
				batchs -= num1
				begin += num1
				flag1 = true
				stats.Malformed++
				break
			}

			if receive[begin+num1] != byte(16) {
				numArray[index] = receive[begin+num1]
			} else {
				num1++
				if num1 >= batchs {
					flag2 = true
					stats.Malformed++
					break
				}
				numArray[index] = receive[begin+num1]
				numArray[index] = numArray[index] ^ byte(255)
			}

			flag3 = false
			num1++
		}

		if flag1 {
			flag1 = false
			continue
		}
		if flag2 {
			break
		}

		siteTo, linkID, siteFrom := decodeDataConvert(numArray)

		// Scan frame end.
		size1 := 0
		flag4 := false
		flag5 := true
		for begin+size1 < len(receive) {
			num2 := receive[begin+size1]
			if !flag5 && num2 == byte(10) {
				flag1 = true
				begin += size1
				// In original code: if to==SendTo.Modem then it logs "Битый пакет..."
				stats.Malformed++
				break
			}
			flag5 = false
			size1++

			// frame end marker 13, and next byte is either 10 or missing.
			if num2 == byte(13) && (size1 == sizeTotal || begin+size1 >= len(receive) || receive[begin+size1] == byte(10)) {
				flag4 = true
				break
			}
		}

		if flag1 {
			flag1 = false
			batchs -= size1
			continue
		}

		if flag4 {
			raw := bytesCopy(receive[begin : begin+size1])
			onFrame(BatchRecord{
				Timestamp: time.Now(),
				ModemID:    modemID,
				ModemType:  modemType,
				SiteIDFrom: siteFrom,
				SiteIDTo:   siteTo,
				LinkID:     linkID,
				BatchHex:   hexByteString(raw),
				Raw:        raw,
			})
			stats.ParsedFrames++
			batchs -= size1
			begin += size1
			continue
		}

		break
	}

	if batchs > 0 && batchs < 10000 {
		remain = bytesCopy(receive[begin : begin+batchs])
		return remain, stats
	}
	return nil, stats
}

