namespace urlShortener.Controllers;


using Microsoft.AspNetCore.Mvc;
using urlShortener.DB;


[ApiController]
[Route("[controller]")]
public class URLController : ControllerBase
{

private  DataContext _dbContext;
    public URLController(DataContext dbContext){
         _dbContext = dbContext;
    }


    public class bodyType {
        public string rawUrl {get;set;} 
    }

    public class URLDPO {
        public string shortenedUrl {get;set;}
    }

   [HttpGet("{id}")]
    public RedirectResult Get(string id){

           var realUrl =  _dbContext.URL.Where(val => val.shortenedUrl.Equals(id)).FirstOrDefault();
        

           if(realUrl is null){
              throw new Exception("No url found");
           }

          return Redirect(realUrl.realUrl);
    }

    [HttpPost(Name = "ShortenUrl")]
    public JsonResult Post([FromBody] bodyType url )
    {

         var validateUrl = System.Text.RegularExpressions.Regex.IsMatch(url.rawUrl,@"^((http|ftp|https|www)://)?([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$");
        
        if(!validateUrl){
            throw new Exception("Url is not valid");
        }

      
            var urlFromDb =  _dbContext.URL.Where(val => val.shortenedUrl.Equals(url.rawUrl)).FirstOrDefault();

                URL urlObject = new URL();
                String UUID = Guid.NewGuid().ToString() ;

                urlObject.realUrl = url.rawUrl;
                urlObject.shortenedUrl = UUID;

                _dbContext.URL.Add(urlObject);

                _dbContext.SaveChanges();

                var result = new URLDPO();

                result.shortenedUrl = $"http://localhost:5001/URL/{UUID}";

                return new JsonResult(result);
    }
}
