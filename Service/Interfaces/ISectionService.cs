using Repository.GetDataFromToken;
using Repository.ViewModel;

namespace Service.Interfaces;

public interface ISectionService
{
    Task<SectionNameViewModel> GetSectionById(int sectionId);
    IEnumerable<SectionNameViewModel> GetSectionList();
    TableListPaginationViewModel GetDiningTablesListBySectionId(int sectionid, int pageNumber, int pageSize, string searchKeyword);
    Task<AuthResponse> AddSection(AddSectionViewModel model);
    Task<AuthResponse> EditSection(AddSectionViewModel model);
    Task<AuthResponse> DeleteSection(int id);
    Task<AuthResponse> AddTable(AddTableViewmodel model);
    Task<AuthResponse> EditTable(AddTableViewmodel model);
    Task<AuthResponse> DeleteTable(int id);
    Task<AuthResponse> DeleteTables(List<int> ids);
}
