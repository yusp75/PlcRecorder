using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using System.Diagnostics;

namespace MyPlc2
{
    public class Db
    {
        private string token = "";
        private const string bucket = "plc";
        private const string org = "yu";
        private InfluxDBClient client;

        private readonly string config_path = AppDomain.CurrentDomain.BaseDirectory + "\\config\\";

        private const int UTC_OFFSET = 8;
        private MyLog log = new(null);

        public Db()
        {
            Connect();
        }

        public void Connect()
        {
            try
            {
                if (token == "")
                {
                    using StreamReader reader = new(config_path + "token");
                    if (reader != null) token = reader.ReadLine();
                }
                if (client is null)
                {
                    client = new InfluxDBClient("http://127.0.0.1:8086", token);
                    //关闭log
                    client.SetLogLevel(InfluxDB.Client.Core.LogLevel.None);                      

                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine("异常：" + ex.Message);
                log.Error(ex.Message);

            }
        }

        public void Write(string name, string address, int pos, string raw_value, double value, int word_len)
        {
            if (client is null) return;

            var mem = new MyTypes
            {
                Name = name,
                Address = address,
                Bit_pos = pos,
                Raw_value = raw_value,
                Value = value,
                WordLen = word_len,
                Time = DateTime.Now
            };

            using (var writeApi = client.GetWriteApi())
            {
                writeApi.WriteMeasurement<MyTypes>(mem, WritePrecision.Ms, bucket, org);
            }
        }

        public async Task<Dictionary<string, MPoint>> Query(string addresses, string start, string stop)
        {
            Dictionary<string, MPoint> record_d = new();

            if (addresses != null && addresses.Length > 0)
            {
                var query = $"from(bucket: \"plc\") " +
                    $"|> range(start: {start}, stop: {stop})" +
                    $"|> filter(fn: (r) => contains(value: r.address, set: {addresses}))" +
                    $"|> pivot(rowKey:[\"_time\"], columnKey: [\"_field\"], valueColumn: \"_value\")" +
                    $"|> drop(columns: [\"_start\", \"_stop\", \"_measurement\"])" +
                    $"|> group(columns: [\"address\"])";

                try
                {
                    var tables = await client.GetQueryApi().QueryAsync(query, org);
                    foreach (var table in tables)
                    {
                        string address = (string)table.Records[0].GetValueByKey("address");
                        MPoint points = new();
                        foreach (var record in table.Records)
                        {
                            try
                            {
                                DateTime t = ((NodaTime.Instant)record.GetValueByKey("_time")).ToDateTimeUtc();
                                t = t.AddHours(UTC_OFFSET);

                                double tt = t.ToOADate();
                                double v = (double)record.GetValueByKey("value");
                                points.AddPoint(tt, v);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("query: " + e);
                                continue;
                            }
                        }
                        record_d[address] = points;
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            return record_d;

        }

        //
    }
}
