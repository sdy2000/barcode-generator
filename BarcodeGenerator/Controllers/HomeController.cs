using BarcodeGenerator.Models;
using BarcodeLib;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace BarcodeGenerator.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult GenerateBarcode(string name = "noName")
        {
            Barcode barcode = new Barcode();
            barcode.IncludeLabel = true;
            barcode.Encode(TYPE.CODE39Extended, name, Color.Black, Color.White, 400, 100);
            Image img = barcode.EncodedImage;
            byte[] data = ConvertImageToByte(img);

            string imageName = $"{Guid.NewGuid().ToString()}.png";
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", imageName);

            if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
                System.IO.File.WriteAllBytes(imagePath, data);
            }

            ViewBag.ImagePath = $"/Images/{imageName}";

            return View();
        }

        private byte[] ConvertImageToByte(Image img)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                img.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }

        public IActionResult GenerateQRCode(
            string name = "noName", string email = "noEmail", string phone = "noPhone"
            )
        {
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            QRCodeData qRCodeData = qRCodeGenerator
                .CreateQrCode(name + "--" + email + "--" + phone, QRCodeGenerator.ECCLevel.Q);
            QRCode qRCode = new QRCode(qRCodeData);
            Bitmap bitmap = qRCode.GetGraphic(10);
            var bitmapBytes = ConvertBitmapToBytes(bitmap);

            string imageName = $"{Guid.NewGuid().ToString()}.png";
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", imageName);

            if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
                System.IO.File.WriteAllBytes(imagePath, bitmapBytes);
            }

            ViewBag.ImagePath = $"/Images/{imageName}";

            return View();
        }

        private byte[] ConvertBitmapToBytes(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
    }
}