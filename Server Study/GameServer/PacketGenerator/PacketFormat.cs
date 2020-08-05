using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    class PacketFormat
    {
        // {0} 패킷 이름
        // {1} 멤버 변수들
        // {2} 멤버 변수 Read
        // {3} 멤버 변수 Write
        public static string packetFormat =
@"
class {0}
{{
    {1}

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;

        ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);       // 공간 예약

        ushort count = 0;       // 자동화하기 편리하게
        bool success = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);     // for Slice

        count += sizeof(ushort);        // 처음 패킷Id
        success &= BitConverter.TryWriteBytes(segment.Slice(count, span.Length - count), (ushort)PacketID.{0});
        count += sizeof(ushort);     // 나중에 자동화
        {3}
        success &= BitConverter.TryWriteBytes(span, count);         // 마지막 최종 카운트
        if (success == false)
            return null;

        return SendBufferHelper.Close(count);
    }}
}}
";

        // {0} 변수 형식
        // {1} 변수 이름
        public static string memberFormat =
@"
public {0} {1};
";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        // {2} 멤버 변수들
        // {3} 멤버 변수 Read
        // {4} 멤버 변수 Write
        public static string memberListFormat =
@"
public struct {0}
{{
    {2}

    public void Read(ReadOnlySpan<byte> span, ref ushort count)
    {{
        {3}
    }}

    public bool Write(Span<byte> span, ref ushort count)
    {{
        bool success = true;
        {4}
        return success;
    }}

    
}}
public List<{0}> {1}s = new List<{0}>();
";

        // {0} 변수 이름
        // {1} To~ 변수 형식
        // {2} 변수 형식
        public static string readFormat =
@"
this.{0} = BitConverter.{1}(span.Slice(count, span.Length - count));
count += sizeof({2});
";

        // {0} 변수 이름
        public static string readStringFormat =
@"
ushort {0}len = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(span.Slice(count, {0}Len));
count += {0}Len;
";
        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        public static string readListFormat =
@"
{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
count += sizeof(ushort);
for (int i = 0; i < {1}Len; i++)
{
    {0} {1} = new {0}();
    {1}.Read(span, ref count);
    {1}.Add({1});
}
";
        // {0} 변수 이름
        // {1} 변수 형식
        public static string writeFormat =
@"
success &= BitConverter.TryWriteBytes(segment.Slice(count, span.Length - count), this.{0});
count += sizeof({1});
";

        // {0} 변수 이름
        public static string writeStringFormat =
@"
ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort))
success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), {0}Len);
count += sizeof(ushort);
count += {0}Len;
";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        public static string writeListFormat =
@"
success &= BitConverter.TryWriteBytes(segment.Slice(count, span.Length - count), (ushort){1}.Count);
count += sizeof(ushort);
foreach ({0} {1} in this.{1}s)
    success &= {1}.Write(span, ref count);
";
    }
}