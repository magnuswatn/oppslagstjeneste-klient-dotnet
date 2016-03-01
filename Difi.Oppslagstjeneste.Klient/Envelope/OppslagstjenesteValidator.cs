﻿using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Difi.Felles.Utility.Exceptions;
using Difi.Felles.Utility.Security;
using Difi.Oppslagstjeneste.Klient.Domene.Enums;
using Difi.Oppslagstjeneste.Klient.Security;

namespace Difi.Oppslagstjeneste.Klient.Envelope
{
    public class Oppslagstjenestevalidator : Responsvalidator
    {
        public OppslagstjenesteInstillinger OppslagstjenesteInstillinger { get; }
        public Miljø Miljø { get; set; }

        public Oppslagstjenestevalidator(XmlDocument sendtDokument, XmlDocument mottattDokument, OppslagstjenesteInstillinger oppslagstjenesteInstillinger, Miljø miljø)
            : base(sendtDokument, mottattDokument, SoapVersion.Soap12, oppslagstjenesteInstillinger.Avsendersertifikat)
        {
            OppslagstjenesteInstillinger = oppslagstjenesteInstillinger;
            Miljø = miljø;
        }

        public void Valider()
        {
            var signedXmlWithAgnosticId = new SignedXmlWithAgnosticId(MottattDokument);
            signedXmlWithAgnosticId.LoadXml(HeaderSignatureElement);

            // Sørger for at motatt envelope inneholder signature confirmation og body samt at id'ne matcher mot header signatur
            ValiderSignaturreferanser(HeaderSignatureElement, signedXmlWithAgnosticId, new[] { "/env:Envelope/env:Header/wsse:Security/wsse11:SignatureConfirmation", "/env:Envelope/env:Body" });

            // Validerer SignatureConfirmation
            PerformSignatureConfirmation(HeaderSecurityElement);

            SjekkTimestamp(TimeSpan.FromSeconds(2000));

            ValiderResponssertifikat(signedXmlWithAgnosticId);
        }

        private void ValiderResponssertifikat(SignedXmlWithAgnosticId signed)
        {
            var signatur = BinaryTokenElement.InnerText;
            var value = Convert.FromBase64String(signatur);
            var sertifikat = new X509Certificate2(value);

            var erGyldigSertifikat = Miljø.Sertifikatkjedevalidator.ErGyldigResponssertifikat(sertifikat);

            if (!erGyldigSertifikat)
            {
                throw new SecurityException("Sertifikatet som er angitt i signaturen er ikke en del av en gyldig sertifikatkjede.");
            }
            var key = sertifikat.PublicKey.Key;
            if (!signed.CheckSignature(key))
               throw new Exception("Signaturen i motatt svar er ikke gyldig");
        }
    }
}
