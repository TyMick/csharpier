using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace CSharpier.VisualStudio
{
    public static class AsyncPackageExtensions
    {
        public static Task<T> GetServiceAsync<T>(this AsyncPackage asyncPackage)
            where T : class
        {
            return asyncPackage.GetServiceAsync<T, T>();
        }
    }
}
