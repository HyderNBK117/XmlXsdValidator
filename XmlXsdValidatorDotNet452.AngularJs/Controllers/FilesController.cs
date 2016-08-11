using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using XmlXsdValidatorDotNet452.AngularJs.Extensions;

namespace XmlXsdValidatorDotNet452.AngularJs.Controllers
{
    public class FilesController : ApiController
    {
        /// <summary>
        ///   Add a photo
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> Post([FromUri]string xml)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }
            try
            {
                var provider = new CustomMultipartFormDataStreamProvider();

                await Request.Content.ReadAsMultipartAsync(provider);

                var result = await IsValid(xml, provider);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        private async Task<Tuple<bool, List<string>>> IsValid(string xml, CustomMultipartFormDataStreamProvider provider)
        {
            var messages = new List<string>();
            var isValid = true;

            var doc = XDocument.Load(new StringReader(xml));

            var schemaSet = new XmlSchemaSet();

            foreach (var item in provider.Contents)
            {
                using (var xsdFile = new StreamReader(await item.ReadAsStreamAsync()))
                {
                    var xsd = XDocument.Load(xsdFile);

                    var targetNamespace = xsd.Elements().First().Attribute("targetNamespace").Value;

                    schemaSet.Add(targetNamespace, xsd.CreateReader());
                }
            }

            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.Schemas = schemaSet;
            settings.ValidationEventHandler += (o, s) =>
            {
                messages.Add($"{s.Severity}: {s.Message}");
                isValid = false;
            };

            using (var xr = XmlReader.Create(doc.CreateReader(), settings))
            {
                while (xr.Read())
                {
                    // Read the file to invoke the validation
                }
            }

            return new Tuple<bool, List<string>>(isValid, messages);
        }
    }
}
