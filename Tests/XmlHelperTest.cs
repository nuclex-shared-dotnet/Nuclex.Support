#region Apache License 2.0
/*
Nuclex .NET Framework
Copyright (C) 2002-2024 Markus Ewald / Nuclex Development Labs

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion // Apache License 2.0

using System;
using System.IO;
using System.Text;
using System.Xml;
#if !USE_XMLDOCUMENT
using System.Xml.Linq;
#endif
using System.Xml.Schema;

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the XML helper class</summary>
  [TestFixture]
  internal class XmlHelperTest {

    /// <summary>A broken XML schema</summary>
    private const string brokenSchemaXml =
      "This is not a valid schema";

    /// <summary>An XML schema with a syntax error</summary>
    private const string syntaxErrorSchemaXml =
      "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
      "<xs:schema" +
      "  elementFormDefault=\"qualified\"" +
      "  xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">" +
      "  <xs:attribute minOccurs=\"0\" maxOccurs=\"2\" name=\"x\" type=\"xs:double\" />" +
      "  <xs:attribute minOccurs=\"0\" maxOccurs=\"2\" name=\"y\" type=\"xs:double\" />" +
      "</xs:schema>";

    /// <summary>A valid XML schema for a list of 2D points</summary>
    private const string pointsSchemaXml =
      "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
      "<xs:schema" +
      "  elementFormDefault=\"qualified\"" +
      "  xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">" +
      "  <xs:complexType name=\"vectorType\">" +
      "    <xs:attribute name=\"x\" type=\"xs:double\" />" +
      "    <xs:attribute name=\"y\" type=\"xs:double\" />" +
      "  </xs:complexType>" +
      "  <xs:complexType name=\"pointsType\">" +
      "    <xs:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">" +
      "      <xs:element name=\"point\" type=\"vectorType\" />" +
      "    </xs:sequence>" +
      "  </xs:complexType>" +
      "  <xs:element name=\"points\" type=\"pointsType\" />" +
      "</xs:schema>";

    /// <summary>A broken XML document</summary>
    private const string brokenXml =
      "This is not a valid XML file";

    /// <summary>
    ///   Well-formed XML document that is not conformant to the schema above
    /// </summary>
    private const string unconformantXml =
      "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
      "<points" +
      "  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
      "  xsi:noNamespaceSchemaLocation=\"skin.xsd\">" +
      "  <point x=\"10\" y=\"20\" z=\"30\" />" +
      "  <point x=\"1\" y=\"2\" />" +
      "</points>";

    /// <summary>Well-formed XML document that is conformant to the schema</summary>
    private const string conformantXml =
      "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
      "<points" +
      "  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
      "  xsi:noNamespaceSchemaLocation=\"skin.xsd\">" +
      "  <point x=\"1\" y=\"2\" />" +
      "  <point x=\"10\" y=\"20\" />" +
      "</points>";

    #region TempFileKeeper

    /// <summary>
    ///   Creates a temporary file and automatically deletes it on dispose
    /// </summary>
    private class TempFileKeeper : IDisposable {

      /// <summary>
      ///   Creates a temporary file with the specified contents using the UTF8 encoding
      /// </summary>
      /// <param name="fileContents">
      ///   Contents that will be written into the temporary file
      /// </param>
      public TempFileKeeper(string fileContents) : this(fileContents, Encoding.UTF8) { }

      /// <summary>Creates a temporary file with the specified contents</summary>
      /// <param name="fileContents">
      ///   Contents that will be written into the temporary file
      /// </param>
      /// <param name="encoding">
      ///   Encoding to use for writing the contents into the file
      /// </param>
      public TempFileKeeper(string fileContents, Encoding encoding) {
        string tempFile = Path.GetTempFileName();
        try {
          using(
            FileStream tempFileStream = new FileStream(
              tempFile, FileMode.Truncate, FileAccess.Write, FileShare.None
            )
          ) {
            StreamWriter writer = new StreamWriter(tempFileStream, encoding);
            writer.Write(fileContents);
            writer.Flush();
          }
        }
        catch(Exception) {
          File.Delete(tempFile);
          throw;
        }

        this.tempFilePath = tempFile;
      }

      /// <summary>Called when the instance is collected by the GC</summary>
      ~TempFileKeeper() {
        Dispose();
      }

      /// <summary>Immediately releases all resources used by the instance</summary>
      public void Dispose() {
        if(this.tempFilePath != null) {
          File.Delete(this.tempFilePath);
          this.tempFilePath = null;

          GC.SuppressFinalize(this);
        }
      }

      /// <summary>Implicitely converts a TempFileKeeper into a file path</summary>
      /// <param name="tempFileKeeper">TempFileKeeper that will be converted</param>
      /// <returns>The path to the temporary file managed by the TempFileKeeper</returns>
      public static implicit operator string(TempFileKeeper tempFileKeeper) {
        return tempFileKeeper.tempFilePath;
      }

      /// <summary>Path to the temporary file the TempFileKeeper is managing</summary>
      private string tempFilePath;

    }

    #endregion // class TempFileKeeper

    /// <summary>
    ///   Verifies that an exception is thrown when a schema fails to load
    /// </summary>
    [Test]
    public void LoadSchemaThrowsOnInvalidSchema() {
      using(
        TempFileKeeper tempFile = new TempFileKeeper(brokenSchemaXml)
      ) {
        Assert.Throws<XmlException>(delegate() { XmlHelper.LoadSchema(tempFile); });
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown when a schema contains a syntax error
    /// </summary>
    [Test]
    public void LoadSchemaThrowsOnSyntaxErrors() {
      using(
        TempFileKeeper tempFile = new TempFileKeeper(syntaxErrorSchemaXml)
      ) {
        Assert.Throws<XmlSchemaException>(delegate() { XmlHelper.LoadSchema(tempFile); });
      }
    }

    /// <summary>
    ///   Verfifies that TryLoadSchema() can fail without throwing an exception
    ///   when the schema is not a valid XML document
    /// </summary>
    [Test]
    public void TryLoadSchemaHandlesMissingFiles() {
      XmlSchema schema;
      Assert.IsFalse(XmlHelper.TryLoadSchema("-- hello world --", out schema));
      Assert.IsNull(schema);
    }

    /// <summary>
    ///   Verfifies that TryLoadSchema() can fail without throwing an exception
    ///   when the schema is not a valid XML document
    /// </summary>
    [Test]
    public void TryLoadSchemaHandlesBrokenSchemas() {
      using(
        TempFileKeeper tempFile = new TempFileKeeper(brokenSchemaXml)
      ) {
        XmlSchema schema;
        Assert.IsFalse(XmlHelper.TryLoadSchema(tempFile, out schema));
        Assert.IsNull(schema);
      }
    }

    /// <summary>
    ///   Verfifies that TryLoadSchema() can fail without throwing an exception
    ///   when the schema contains a syntax error
    /// </summary>
    [Test]
    public void TryLoadSchemaHandlesSyntaxErrors() {
      using(
        TempFileKeeper tempFile = new TempFileKeeper(syntaxErrorSchemaXml)
      ) {
        XmlSchema schema;
        Assert.IsFalse(XmlHelper.TryLoadSchema(tempFile, out schema));
        Assert.IsNull(schema);
      }
    }

    /// <summary>Tests whether a normal, valid schema can be loaded successfully</summary>
    [Test]
    public void SchemasCanBeLoadedFromFiles() {
      using(
        TempFileKeeper tempFile = new TempFileKeeper(pointsSchemaXml)
      ) {
        XmlHelper.LoadSchema(tempFile);
      }
    }

    /// <summary>Tests whether a normal, valid schema can be loaded successfully</summary>
    [Test]
    public void TryLoadSchemaCanLoadSchemas() {
      using(
        TempFileKeeper tempFile = new TempFileKeeper(pointsSchemaXml)
      ) {
        XmlSchema schema;
        Assert.IsTrue(XmlHelper.TryLoadSchema(tempFile, out schema));
        Assert.NotNull(schema);
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown when an invalid XML document is loaded
    /// </summary>
    [Test]
    public void LoadingInvalidDocumentThrows() {
      using(TextReader schemaReader = new StringReader(pointsSchemaXml)) {
        XmlSchema schema = XmlHelper.LoadSchema(schemaReader);
        using(
          TempFileKeeper tempFile = new TempFileKeeper(brokenXml)
        ) {
          Assert.Throws<XmlException>(
            delegate() { XmlHelper.LoadDocument(schema, tempFile); }
          );
        }
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown when a nonconformant XML document is loaded
    /// </summary>
    [Test]
    public void SchemaValidationFailureCausesException() {
      using(TextReader schemaReader = new StringReader(pointsSchemaXml)) {
        XmlSchema schema = XmlHelper.LoadSchema(schemaReader);
        using(
          TempFileKeeper tempFile = new TempFileKeeper(unconformantXml)
        ) {
          Assert.Throws<XmlSchemaValidationException>(
            delegate() { XmlHelper.LoadDocument(schema, tempFile); }
          );
        }
      }
    }

    /// <summary>
    ///   Tests whether a normal, conformant XML document can be loaded successfully
    /// </summary>
    [Test]
    public void LoadedDocumentIsValidatedAgainstSchema() {
      using(TextReader schemaReader = new StringReader(pointsSchemaXml)) {
        XmlSchema schema = XmlHelper.LoadSchema(schemaReader);
        using(
          TempFileKeeper tempFile = new TempFileKeeper(conformantXml)
        ) {
#if USE_XMLDOCUMENT
          XmlDocument document = XmlHelper.LoadDocument(schema, tempFile);
#else
          XDocument document = XmlHelper.LoadDocument(schema, tempFile);
#endif
        }
      }
    }

  }

} // namespace Nuclex.Support
