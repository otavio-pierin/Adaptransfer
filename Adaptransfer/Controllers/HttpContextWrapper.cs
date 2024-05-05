namespace Adaptransfer.Controllers
{
    internal class HttpContextWrapper
    {
        private object context;

        public HttpContextWrapper(object context) {
            this.context = context;
        }
    }
}