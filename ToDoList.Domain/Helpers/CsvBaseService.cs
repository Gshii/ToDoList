using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace ToDoList.Domain.Helpers;

public class CsvBaseService<T>
{
    private readonly CsvConfiguration _csvConfiguration;
    
    public CsvBaseService()
    {
        _csvConfiguration = GetConfiguration();
    }

    public byte[] UploadFile(IEnumerable<T> data)
    {
        using(var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(streamWriter, _csvConfiguration))
        {
            streamWriter.Write('\uFEFF');
            csvWriter.WriteRecords(data);
            streamWriter.Flush();
            return memoryStream.ToArray();
        }    
    }

    public CsvConfiguration GetConfiguration()
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Encoding = Encoding.UTF8,
            NewLine = "\r\n"
        };
    }
}