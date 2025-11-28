using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Lab3OOP.Model;
using Contact = Lab3OOP.Model.Contact;

namespace Lab3OOP.Services
{
    public class JsonContactsService
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
        };

        public async Task<List<Contact>> LoadAsync(string path)
        {
            if (!File.Exists(path)) return new List<Contact>();

            try
            {
                using var stream = File.OpenRead(path);
                var result = await JsonSerializer.DeserializeAsync<List<Contact>>(stream, _options);
                return result ?? new List<Contact>();
            }
            catch
            {
                return new List<Contact>();
            }
        }

        public async Task SaveAsync(string path, IEnumerable<Contact> contacts)
        {
            using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, contacts, _options);
        }
    }
}
