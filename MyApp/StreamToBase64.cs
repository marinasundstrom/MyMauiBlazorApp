namespace MyApp;

public class StreamToBase64
{
    public static string ConvertStreamToBase64(Stream inputStream)
    {
        // Ensure the stream is at the beginning
        if (inputStream.CanSeek)
        {
            inputStream.Seek(0, SeekOrigin.Begin);
        }

        using (var memoryStream = new MemoryStream())
        {
            // Copy the input stream to the memory stream
            inputStream.CopyTo(memoryStream);

            // Get the byte array from the memory stream
            byte[] byteArray = memoryStream.ToArray();

            // Convert the byte array to a Base64 string
            string base64String = Convert.ToBase64String(byteArray);

            return base64String;
        }
    }
}