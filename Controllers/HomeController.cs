using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using CMCS_PROG6212_POE.Data;

namespace CMCS_PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // Home page
        public IActionResult Index()
        {
            return View();
        }

        // Sign-Up page (for Program Coordinators and Academic Managers)
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user)
        {
            // Ensure that only Program Coordinators or Academic Managers can sign up
            if (user.Role == "Program Coordinator" || user.Role == "Academic Manager")
            {
                // Save the user to the database
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            // If the role is invalid, show an error
            ModelState.AddModelError("", "Only Program Coordinators and Academic Managers can sign up.");
            return View(user);
        }

        // Login page
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Store user information in the session
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);

                // Redirect to Verify Claims page after login
                return RedirectToAction("VerifyClaims");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();  // This should return the Privacy view
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Privacy(ClaimModel claim, IFormFile file)
        {
            // Log ModelState validation check
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("ModelState Error: " + error.ErrorMessage);
                }
                return View(claim);
            }

            // Handle file upload
            if (file != null && file.Length > 0)
            {
                // Define the directory path
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                // Create the directory if it does not exist
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                // Store the uploaded file
                var filePath = Path.Combine(uploadsDir, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Set the file name on the claim object
                claim.FileName = file.FileName;
            }
            else
            {
                ModelState.AddModelError("FileName", "File must be uploaded.");
                return View(claim);  // Return the view if no file is uploaded
            }

            try
            {
                // Set the default status to Pending
                claim.Status = "Pending";

                // Add the claim to the database context
                Console.WriteLine($"Adding claim for: {claim.FirstName} {claim.LastName}, Hours Worked: {claim.HoursWorked}");
                _context.Claims.Add(claim);

                // Save the changes asynchronously to the database
                await _context.SaveChangesAsync();
                Console.WriteLine("Claim successfully saved to the database.");

                // Provide feedback to the user that the claim was successfully submitted
                TempData["Message"] = "Claim submitted successfully.";

                // Redirect to TrackClaims action after submission
                return RedirectToAction(nameof(TrackClaims), new { firstName = claim.FirstName, lastName = claim.LastName });
            }
            catch (Exception ex)
            {
                // Log the exception to help with debugging
                Console.WriteLine("Error while saving claim: " + ex.Message);

                // Add a model error and return the view to display the error
                ModelState.AddModelError("", "There was an error submitting your claim. Please try again.");
                return View(claim);
            }
        }



        [HttpGet]
        public IActionResult VerifyClaims()
        {
            // Fetch all claims, regardless of status
            var claims = _context.Claims.ToList();

            if (!claims.Any())
            {
                ViewBag.Message = "No claims to verify.";
            }

            return View(claims);
        }

        public IActionResult TrackClaims(string firstName, string lastName)
        {
            var claims = _context.Claims.Where(c => c.FirstName == firstName && c.LastName == lastName).ToList();

            if (!claims.Any())
            {
                ViewBag.Message = $"No claims found for {firstName} {lastName}.";
            }

            return View(claims);
        }



        [HttpPost]
        public IActionResult ApproveClaim(int claimId)
        {
            var claim = _context.Claims.Find(claimId);
            if (claim != null)
            {
                claim.Status = "Approved";
                _context.SaveChanges();
                TempData["Message"] = $"Claim {claimId} has been approved.";
            }
            return RedirectToAction("VerifyClaims");
        }

        [HttpPost]
        public IActionResult RejectClaim(int claimId)
        {
            var claim = _context.Claims.Find(claimId);
            if (claim != null)
            {
                claim.Status = "Rejected";
                _context.SaveChanges();
                TempData["Message"] = $"Claim {claimId} has been rejected.";
            }
            return RedirectToAction("VerifyClaims");
        }

        // Logout action
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
