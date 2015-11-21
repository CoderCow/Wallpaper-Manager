using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Windows;
using WallpaperManager.Models;
using Path = Common.IO.Path;

namespace UnitTests.Models {
  internal class AppDataReaderWriterFake : AppDataReaderWriter {
    public string WrittenOutputString { get; private set; }
    public bool UseOutputDataAsInput { get; set; }

    public AppDataReaderWriterFake(Path inputFilePath, IDisplayInfo displayInfo) : base(inputFilePath, displayInfo) {}

    protected override Stream GetOutputStream() {
      return new MemoryStream();
    }

    protected override Stream GetInputStream() {
      if (!this.UseOutputDataAsInput) {
        return base.GetInputStream();
      } else {
        MemoryStream inputStream = new MemoryStream(this.WrittenOutputString.Length);

        using (StreamWriter writer = new StreamWriter(inputStream, Encoding.UTF8, 1024, true))
          writer.Write(this.WrittenOutputString);

        inputStream.Position = 0;

        return inputStream;
      }
    }

    protected override void WriteInternal(Stream outputStream, IApplicationData appData) {
      base.WriteInternal(outputStream, appData);

      outputStream.Position = 0;

      StreamReader reader = new StreamReader(outputStream);
      this.WrittenOutputString = reader.ReadToEnd();
    }
  }
}
