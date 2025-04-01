using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Kitbox_API.DTOs;
using Kitbox_API.Models;
using Kitbox_API.Repositories;

namespace Kitbox_API.Services
{
    public class CabinetService : ICabinetService
    {
        private readonly ICabinetRepository _cabinetRepository;
        private readonly IMapper _mapper;

        public CabinetService(ICabinetRepository cabinetRepository, IMapper mapper)
        {
            _cabinetRepository = cabinetRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CabinetDto>> GetAllCabinetsAsync()
        {
            var cabinets = await _cabinetRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CabinetDto>>(cabinets);
        }

        public async Task<CabinetDto> GetCabinetByIdAsync(int id)
        {
            var cabinet = await _cabinetRepository.GetByIdAsync(id);
            return _mapper.Map<CabinetDto>(cabinet);
        }

        public async Task<CabinetDto> CreateCabinetAsync(CabinetCreateDto cabinetDto)
        {
            var cabinet = _mapper.Map<Cabinet>(cabinetDto);
            var addedCabinet = await _cabinetRepository.AddAsync(cabinet);
            await _cabinetRepository.SaveChangesAsync();
            return _mapper.Map<CabinetDto>(addedCabinet);
        }

        public async Task UpdateCabinetAsync(int id, CabinetUpdateDto cabinetDto)
        {
            var cabinet = await _cabinetRepository.GetByIdAsync(id);
            if (cabinet == null)
                throw new KeyNotFoundException($"Cabinet with ID {id} not found");
                
            _mapper.Map(cabinetDto, cabinet);
            await _cabinetRepository.UpdateAsync(cabinet);
            await _cabinetRepository.SaveChangesAsync();
        }

        public async Task DeleteCabinetAsync(int id)
        {
            var cabinet = await _cabinetRepository.GetByIdAsync(id);
            if (cabinet == null)
                throw new KeyNotFoundException($"Cabinet with ID {id} not found");
                
            await _cabinetRepository.DeleteAsync(cabinet);
            await _cabinetRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CabinetDto>> GetCabinetsByOrderIdAsync(int orderId)
        {
            var cabinets = await _cabinetRepository.GetCabinetsByOrderIdAsync(orderId);
            return _mapper.Map<IEnumerable<CabinetDto>>(cabinets);
        }
    }
}