using InfluxDB.Client.Core;

namespace MyPlc2
{
    [Measurement("mytypes")]
    public class MyTypes
    {
        //device,name,address 1,areas 2,number 3,start 4,data_type 5,delay->cycle 6,size 7, bit_pos 8

        [Column("name", IsTag = true)] public string? Name { get; set; }
        [Column("address", IsTag = true)] public string? Address { get; set; }
        [Column("bit_pos")] public int Bit_pos { get; set; }
        [Column("raw_value")] public string? Raw_value { get; set; }
        [Column("value")] public double Value { get; set; }
        [Column("WordLen")] public int WordLen { get; set; } //数据类型 data_type
        [Column(IsTimestamp = true)] public DateTime Time { get; set; }


    }

    //
}
