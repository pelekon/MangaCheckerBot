using System.Threading.Tasks;

namespace MangaChecker.Core.Defines.Parser
{
    public interface ISourceDataParser
    {
        Task<SourceDataParserResult> ParseSourceContent(string content, bool isUpdate);
    }
}