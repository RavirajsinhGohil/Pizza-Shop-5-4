using Repository.ViewModel;
using Service.Interfaces;
using Repository.Interfaces;
using Repository.GetDataFromToken;

namespace Service.Implementations;

public class SectionService : ISectionService
{
    private readonly ISectionRepository _sectionRepository;
    public SectionService(ISectionRepository sectionRepository)
    {
       _sectionRepository = sectionRepository;
    }

    public async Task<SectionNameViewModel> GetSectionById(int sectionId)
    {
        return await _sectionRepository.GetSectionById(sectionId);
    }

    public IEnumerable<SectionNameViewModel> GetSectionList()
    {
        return _sectionRepository.GetSectionList();
    }

    public TableListPaginationViewModel GetDiningTablesListBySectionId(int sectionid, int pageNumber, int pageSize, string searchKeyword)
    {
        return _sectionRepository.GetDiningTablesListBySectionId(sectionid, pageNumber, pageSize, searchKeyword);
    }

    public async Task<AuthResponse> AddSection(AddSectionViewModel model)
    {
        return await _sectionRepository.AddSection(model);
    }

    public async Task<AuthResponse> EditSection(AddSectionViewModel model)
    {
        return await _sectionRepository.EditSection(model);
    }

    public async Task<AuthResponse> DeleteSection(int id)
    {
        return await _sectionRepository.DeleteSection(id);
    }

    public async Task<AuthResponse> AddTable(AddTableViewmodel model)
    {
        return await _sectionRepository.AddTable(model);
    }

    public async Task<AuthResponse> EditTable(AddTableViewmodel model)
    {
        return await _sectionRepository.EditTable(model);
    }

    public async Task<AuthResponse> DeleteTable(int id)
    {
        return await _sectionRepository.DeleteTable(id);
    }
    
    public async Task<AuthResponse> DeleteTables(List<int> ids)
    {
        return await _sectionRepository.DeleteTables(ids);
    }
}
