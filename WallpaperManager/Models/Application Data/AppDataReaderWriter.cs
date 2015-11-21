using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using Common.Windows;
using Microsoft.Practices.ServiceLocation;
using Path = Common.IO.Path;

namespace WallpaperManager.Models {
  public class AppDataReaderWriter : IApplicationDataReader, IApplicationDataWriter {
    public const int CurrentDataVersion = 1;

    private readonly DataContractSerializer serializer;
    private readonly Path inputFilePath;
    private readonly IDisplayInfo displayInfo;

    /// <param name="inputFilePath">
    ///   The <see cref="Path" /> of the XML-file to be read from.
    /// </param>
    /// <param name="displayInfo">
    ///   The display information provider used to verify and correct screen settings.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <paramref name="inputFilePath" /> is <c>Path.Invalid</c>.
    /// </exception>
    public AppDataReaderWriter(Path inputFilePath, IDisplayInfo displayInfo) {
      Contract.Requires<ArgumentException>(inputFilePath != Path.Invalid);
      Contract.Requires<ArgumentNullException>(displayInfo != null);

      this.inputFilePath = inputFilePath;
      this.displayInfo = displayInfo;

      this.serializer = new DataContractSerializer(typeof(IApplicationData), new DataContractSerializerSettings {
        SerializeReadOnlyTypes = false,
        KnownTypes = new[] {
          typeof(ApplicationData),
          typeof(Configuration),
          typeof(ScreenSettings),
          typeof(TextOverlay),
          typeof(WallpaperCategory),
          typeof(WallpaperDefaultSettings),
          typeof(SyncedWallpaperCategory),
          typeof(WallpaperDefaultSettings),
          typeof(WallpaperBase),
          typeof(Wallpaper)
        }
      });
    }

    public IApplicationData Read() {
      try {
        using (Stream inputStream = this.GetInputStream())
          return this.ReadInternal(inputStream);
      } catch (IOException) {
        throw;
      } catch (Exception ex) {
        throw new IOException("An I/O error has occured. See inner exception for details.", ex);
      }
    }

    public void Write(IApplicationData appData) {
      try {
        using (Stream outputStream = this.GetOutputStream()) {
          appData.DataVersion = CurrentDataVersion;
          this.WriteInternal(outputStream, appData);
        }
      } catch (IOException) {
        throw;
      } catch (Exception ex) {
        throw new IOException("An I/O error has occured. See inner exception for details.", ex);
      }
    }

    protected virtual IApplicationData ReadInternal(Stream inputStream) {
      return (IApplicationData)this.serializer.ReadObject(inputStream);
    }

    protected virtual void WriteInternal(Stream outputStream, IApplicationData appData) {
      this.serializer.WriteObject(outputStream, appData);
    }

    protected virtual Stream GetInputStream() {
      return new FileStream(this.inputFilePath, FileMode.Open);
    }

    protected virtual Stream GetOutputStream() {
      return new FileStream(this.inputFilePath, FileMode.Create);
    }
  }
}
