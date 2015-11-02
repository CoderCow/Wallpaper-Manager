// This source is subject to the Creative Commons Public License.
// Please see the README.MD file for more information.
// All other rights reserved.

#define PotGenerator
#define DEBUG
#if PotGenerator && DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace WallpaperManager {
  // Immediate Window Call:
  // WallpaperManager.PotGenerator.Generate("C:\\Cygwin\\usr\\tmp\\Wallpaper Manager\\Wallpaper Manager.pot");
  public static class PotGenerator {
    public static void Generate(string filePath) {
      var excludedEntryNames = new List<string> {"Translation.LastUpdateDate"};
      MemoryStream fileStream = new MemoryStream();

      using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8)) {
        // Header
        writer.WriteLine("# Wallpaper Manager Localization Template File");
        writer.WriteLine("# Copyright (C) 2010, David-Kay Posmyk");
        writer.WriteLine("# This file is distributed under the same license as Wallpaper Manager.");
        writer.WriteLine("# David-Kay Posmyk <KayPosmyk@gmx.de>, 2010");
        writer.WriteLine("# ");
        writer.WriteLine("msgid \"\"");
        writer.WriteLine("msgstr \"\"");
        writer.WriteLine("\"Project-Id-Version: " + Assembly.GetCallingAssembly().GetName().Version + "\\n\"");
        writer.WriteLine("\"Report-Msgid-Bugs-To: KayPosmyk@gmx.de\\n\"");
        writer.WriteLine("\"POT-Creation-Date: 2011-01-05 18:00+0100\\n\"");
        writer.WriteLine("\"PO-Revision-Date: YEAR-MO-DA HO:MI+ZONE\\n\"");
        writer.WriteLine("\"Last-Translator: David-Kay Posmyk <KayPosmyk@gmx.de>\\n\"");
        writer.WriteLine("\"Language-Team: Template <Tem@plate.org>\\n\"");
        writer.WriteLine("\"MIME-Version: 1.0\\n\"");
        writer.WriteLine("\"Content-Type: text/plain; charset=utf-8\\n\"");
        writer.WriteLine("\"Content-Transfer-Encoding: 8bit\\n\"");
        writer.WriteLine();

        ResXResourceReader resxReader = new ResXResourceReader(
          @"G:\Projects\C#\Freeware\Wallpaper Manager\wallpaperman\Wallpaper Manager\Views\Resources\Localization\LocalizationData.resx");
        resxReader.UseResXDataNodes = true;

        foreach (DictionaryEntry entry in resxReader) {
          ResXDataNode node = entry.Value as ResXDataNode;

          if (node != null) {
            string value = (node.GetValue((AssemblyName[])null) as string);
            string comment = node.Comment;

            if (value != null && !excludedEntryNames.Contains(node.Name))
              PotGenerator.WriteEntry(writer, value, node.Name, comment);
          }
        }

        /*using (FileStream fileWriter = new FileStream(filePath, FileMode.Create)) {
          // Skip first bytes since the pot will be invalid in the usual windows format.
          fileStream.Position = 3;

          while (fileStream.Position < fileStream.Length - 1) {
            fileWriter.WriteByte((Byte)fileStream.ReadByte());
          }
        }*/
      }
    }

    private static void WriteEntry(StreamWriter writer, string originalText, string entryName, string comments) {
      string context = @"LocalizationData.resx";
      string commentsNew = "Entry Name: " + entryName + ".";

      string[] commentLines = comments.Split(new[] {"\\n"}, StringSplitOptions.None);
      foreach (string commentLine in commentLines) {
        // Take screenshot comment lines as context.
        if (commentLine.StartsWith("(Screenshot: ")) {
          context = commentLine;
          continue;
        }

        commentsNew += " " + commentLine.Replace("\"", "'");
      }
      if (commentsNew.Length >= 255)
        throw new FormatException("Generated comment line exceeds 255 chars maximum.");

      writer.WriteLine("#: " + context);
      writer.WriteLine("msgctxt \"" + commentsNew + "\"");

      writer.Write("msgid \"");
      writer.Write(originalText);
      writer.WriteLine("\"");
      writer.WriteLine("msgstr \"\"");
      writer.WriteLine();
    }
  }
}

#endif