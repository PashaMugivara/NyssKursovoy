
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
                if (whereShifr == "Шифровать") Text.OutFile = Text.Encrypt();
                if (whereShifr == "Дешифровать") Text.OutFile = Text.Decrypt();
            }
            if (action == "Сохранить в файл")
            {

                string fullpath = Path.Combine(_env.WebRootPath, "Files/output.txt");
                using (var fileStream = new StreamWriter(fullpath, false))
                {
                    fileStream.Write(Text.OutFile);
                    fileStream.Close();

                }
            }

            return RedirectToAction("List");
        }



    }
}