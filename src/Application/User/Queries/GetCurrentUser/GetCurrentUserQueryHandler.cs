﻿using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.User.Queries.GetCurrentUser;

public class
    GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQueryRequest, ServiceResult<GetCurrentUserQueryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(IApplicationDbContext context,
        IIdentityService identityService, ICurrentUserService currentUserService, IMapper mapper)
    {
        _context = context;
        _identityService = identityService;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<ServiceResult<GetCurrentUserQueryResponse>> Handle(GetCurrentUserQueryRequest request,
        CancellationToken cancellationToken)
    {
        string? currentUserId = _currentUserService.UserId;

        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new UnauthorizedAccessException();
        }
        
        UserInfo? userInfo = _context.UserInfos.FirstOrDefault(u => u.Id == currentUserId);

        GetCurrentUserQueryResponse? result = _mapper.Map<GetCurrentUserQueryResponse>(userInfo);
        
        IList<string> roles = await _identityService.GetUserRolesAsync(currentUserId);
        
        result.Roles = roles;

        return ServiceResult.Success(result);
    }
}