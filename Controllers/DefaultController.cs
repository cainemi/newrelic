using System;
using System.Threading.Tasks;
using System.Web.Http;

public class DefaultController : ApiController
{
    [CustomActionFilter]
    [HttpGet]
    public async Task<IHttpActionResult> HelloWorld()
    {
        // Force garbage collection and wait for finalizers
        GC.Collect();
        GC.WaitForPendingFinalizers();

        return Ok("Hello, world!");
    }
}
