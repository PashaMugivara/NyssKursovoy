
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NyssKursovoy.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace NyssKursovoy.Controllers
{
    public class FilesController : Controller
    {
        private IHostingEnvironment _env;
        public FilesController(IHostingEnvironment env)
        {
            _env = env;
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public IActionResult List(IFormFile shifrFile, string action, string whereShifr, string key, string Content)
        {
            Text.Key = key;
            Text.InFile = Content;
            if(action == "Загрузить файл") 
            {
                Text.InFile = "";
                if (shifrFile != null && shifrFile.Length > 0)
                {
                    if (shifrFile.FileName.Substring(shifrFile.FileName.Length - 4) == "docx")
                    {
                        string fullpath = Path.Combine(_env.WebRootPath, "Files/input.docx");
                        using (var fileStream = new FileStream(fullpath, FileMode.Create))
                        {
                            shifrFile.CopyToAsync(fileStream);
                            fileStream.Close();

                        }
                        Text.InFile = Text.TextFromWord("wwwroot/Files/input.docx");
                    }
                    else if (shifrFile.FileName.Substring(shifrFile.FileName.Length - 3) == "txt")
                    {
                        string fullpath = Path.Combine(_env.WebRootPath, "Files/input.txt");
                        using (var fileStream = new FileStream(fullpath, FileMode.Create))
                        {
                            shifrFile.CopyToAsync(fileStream);
                            fileStream.Close();

                        }
                        using (StreamReader sr = new StreamReader("wwwroot/Files/input.txt"))
                        {
                            Text.InFile = sr.ReadToEnd();
                            sr.Close();
                        }
                    }
                }
            }
            if (action == "Преобразовать")
            {
                if (string.IsNullOrWhiteSpace(Text.Key)) Text.OutFile = Text.InFile;
                else if (string.IsNullOrWhiteSpace(Text.InFile)) Text.OutFile="";
                else if (whereShifr == "Шифровать") Text.OutFile = Text.Encrypt();
                else if (whereShifr == "Дешифровать") Text.OutFile = Text.Decrypt();
                string fullpath = Path.Combine(_env.WebRootPath, "Files/output.txt");
                using (var fileStream = new StreamWriter(fullpath, false))
                {
                    fileStream.Write(Text.OutFile);
                    fileStream.Close();
                }
                fullpath = Path.Combine(_env.WebRootPath, "Files/output.docx");
                using (var fileStream = new StreamWriter(fullpath, false))
                {
                    fileStream.Write(Text.OutFile);
                    fileStream.Close();
                }
            }
            if (action == "docx Файл")
            {
                return RedirectToAction("output.docx");

            }

            return RedirectToAction("List");
        }



    }
}