package itmcom

import (
	"encoding/hex"
	"fmt"
	"strings"
)

func hexByteString(data []byte) string {
	// Match original: "{0:X2} " concatenated (uppercase hex + trailing space).
	var b strings.Builder
	b.Grow(len(data) * 3)
	for _, v := range data {
		b.WriteString(strings.ToUpper(fmt.Sprintf("%02X ", v)))
	}
	return b.String()
}

func normalizeNameVSBytes(b []byte) string {
	// Original C# NormalizeNameVS: str.Remove(str.IndexOf(char.MinValue))
	// char.MinValue is '\0'. In ASCII encoding it maps to 0 byte.
	for i, v := range b {
		if v == 0x00 {
			return string(b[:i])
		}
	}
	return string(b)
}

func normalizeNameFixedLen20(s string) []byte {
	// Original C# Utils.NormalizeName: fixed-length string padded with '\0' (char.MinValue).
	// Server sends raw ASCII bytes of that normalized 20-char string.
	out := make([]byte, 20)
	for i := 0; i < 20; i++ {
		if i < len(s) {
			out[i] = s[i]
		} else {
			out[i] = 0
		}
	}
	return out
}

func decodeDataConvert(header9 [9]byte) (sideIDTo int, linkID byte, sideIDFrom int) {
	// In C# DataConvert.Convert does ViceVersa (swap bytes) for each ushort:
	// sideIdTo bytes are at offsets 1 and 2 (little-endian struct in memory),
	// so after swap the semantic becomes big-end decode.
	// sideIdFrom bytes are at offsets 4 and 5.
	// counter also swapped, but it's unused by our MVP.
	sideIDTo = int(uint16(header9[1])<<8 | uint16(header9[2]))
	linkID = header9[3]
	sideIDFrom = int(uint16(header9[4])<<8 | uint16(header9[5]))
	return
}

func bytesCopy(buf []byte) []byte {
	if len(buf) == 0 {
		return nil
	}
	out := make([]byte, len(buf))
	copy(out, buf)
	return out
}

func encodeBytesHexNoSpaces(data []byte) string {
	return hex.EncodeToString(data)
}

func isATSWPCommand(batchType byte) bool {
	return batchType > 5
}

// wrapATSWPBatch mirrors ITMCOMDLL.CorrectBatchContent for outgoing writes.
// batchType 0..5 are data channels, 196+ are command/config/control.
func wrapATSWPBatch(oldBatch []byte, batchType byte) []byte {
	out := make([]byte, 0, len(oldBatch)+8)
	out = append(out, 192)
	out = append(out, batchType)

	// placeholder size (big-endian)
	out = append(out, 0, 0)
	isData := batchType <= 5
	if isData {
		// placeholder for full frame size on positions [4],[5]
		out = append(out, 0, 0)
	}

	for _, b := range oldBatch {
		if isData && (b == 192 || b == 193 || b == 195) {
			out = append(out, 195, b|32)
		} else {
			out = append(out, b)
		}
	}
	out = append(out, 193)

	n := len(out)
	hi := byte((n >> 8) & 0xFF)
	lo := byte(n & 0xFF)
	if isData {
		out[4] = hi
		out[5] = lo
	} else {
		out[2] = hi
		out[3] = lo
	}
	return out
}

