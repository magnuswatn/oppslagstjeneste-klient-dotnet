﻿using System.Xml;
using Difi.Oppslagstjeneste.Klient.Domene;

namespace Difi.Oppslagstjeneste.Klient.Envelope
{
    internal abstract class AbstractEnvelope
    {
        protected AbstractEnvelope()
        {
            Settings = new EnvelopeSettings();
        }

        internal EnvelopeSettings Settings { get; set; }

        protected XmlDocument Document { get; set; }

        public XmlDocument XmlDocument
        {
            get
            {
                if (Document == null)
                {
                    InitializeXmlDocument();
                }
                return Document;
            }
        }

        protected virtual XmlElement CreateHeader()
        {
            return Document.CreateElement("soap", "Header", Navnerom.SoapEnvelope12);
        }

        protected virtual XmlElement CreateBody()
        {
            return Document.CreateElement("soap", "Body", Navnerom.SoapEnvelope12);
        }

        /// <summary>
        ///     Metoden kalles etter at header og body er generert ferdig.
        /// </summary>
        /// <param name="node">Header elementet.</param>
        protected virtual void AddSignatureToHeader(XmlNode node)
        {
        }

        private void InitializeXmlDocument()
        {
            Document = new XmlDocument {PreserveWhitespace = true};
            var baseNode = Document.CreateElement("soap", "Envelope", Navnerom.SoapEnvelope12);
            Document.AppendChild(baseNode);
            var xmlDeclaration = Document.CreateXmlDeclaration("1.0", "UTF-8", null);
            Document.InsertBefore(xmlDeclaration, Document.DocumentElement);
            var header = CreateHeader();
            Document.DocumentElement.AppendChild(header);
            Document.DocumentElement.AppendChild(CreateBody());
            AddSignatureToHeader(header);
        }
    }
}