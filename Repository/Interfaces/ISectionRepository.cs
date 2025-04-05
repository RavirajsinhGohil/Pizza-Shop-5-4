using Repository.GetDataFromToken;
using Repository.ViewModel;

namespace Repository.Interfaces;

public interface ISectionRepository
{
    Task<SectionNameViewModel> GetSectionById(int sectionId);
    IEnumerable<SectionNameViewModel> GetSectionList();
    TableListPaginationViewModel GetDiningTablesListBySectionId(int sectionid, int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    Task<AuthResponse> AddSection(AddSectionViewModel model);
    Task<AuthResponse> EditSection(AddSectionViewModel model);
    Task<AuthResponse> DeleteSection(int id);
    Task<AuthResponse> AddTable(AddTableViewmodel model);
    Task<AuthResponse> EditTable(AddTableViewmodel model);
    Task<AuthResponse> DeleteTable(int id);
    Task<AuthResponse> DeleteTables(List<int> ids);

}