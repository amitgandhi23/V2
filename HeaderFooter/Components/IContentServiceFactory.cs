using LendingTree.Web.ApplicationServices;

namespace LendingTree.Web.Components
{
    public interface IContentServiceFactory
    {
        IContentService GetContentService();
    }
}
