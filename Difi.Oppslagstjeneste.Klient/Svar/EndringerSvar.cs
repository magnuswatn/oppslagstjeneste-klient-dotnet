﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Difi.Oppslagstjeneste.Klient.Domene.Exceptions;
using Difi.Oppslagstjeneste.Klient.DTO;
using Person = Difi.Oppslagstjeneste.Klient.Domene.Entiteter.Person;

namespace Difi.Oppslagstjeneste.Klient.Svar
{
    /// <summary>
    ///     Response sendt fra Oppslagstjensten for å levere ut endringer fra kontakt og reservasjonsregisteret til Virksomhet
    /// </summary>
    public class EndringerSvar : Svar
    {
        public EndringerSvar(XmlDocument xmlDocument)
            : base(xmlDocument)
        {
        }

        public EndringerSvar()
        {
        }

        /// <summary>
        ///     Et endringsNummer, et nummer som identifiserer en endring i et register.
        /// </summary>
        /// <remarks>
        ///     Enhver endring i Oppslagstjenesten tilordnes ett løpenummer som øker med en (1) for hver endring. Dette brukes for
        ///     at virksomheter som vedlikeholder
        ///     lokale kopier av registeret enkelt skal ha sporbarhet på alle endringer.
        /// </remarks>
        public long FraEndringsNummer { get; set; }

        /// <summary>
        ///     Et endringsNummer, et nummer som identifiserer en endring i et register. Dersom TilEndringsNummer er ulik fra
        ///     SenesteEndringsNummer så bør Offentlig Virksomhet sende ny HentEndringer forespørsel
        /// </summary>
        /// <remarks>
        ///     Dersom TilEndringsNummer og SenesteEndringsNummer er lik finnes det ikke fler endringer i registeret som ikke er
        ///     utlevert.
        /// </remarks>
        public long TilEndringsNummer { get; set; }

        /// <summary>
        ///     Beskriver siste endringsnummer i et register. Kan sammenlignes med tilEndringsNummer for å vite om det finnes flere
        ///     endringer i registeret.
        /// </summary>
        /// <remarks>
        ///     Dersom TilEndringsNummer og SenesteEndringsNummer er lik finnes det ikke fler endringer i registeret som ikke er
        ///     utlevert.
        /// </remarks>
        public long SenesteEndringsNummer { get; set; }

        /// <summary>
        ///     Person er en Innbygger utlevert fra kontakt og reservasjonsregisteret.
        /// </summary>
        public IEnumerable<Person> Personer { get; set; }

        protected override void ParseTilKlassemedlemmer()
        {
            var responseElement =
                XmlDocument.SelectSingleNode("/env:Envelope/env:Body", XmlNamespaceManager)
                    as XmlElement;
            
            var deserializedResponse = SerializeUtil.Deserialize<HentEndringerRespons>(responseElement.InnerXml);
            Personer = DtoKonverterer.TilDomeneObjekt(deserializedResponse).Personer;
        }
    }
}