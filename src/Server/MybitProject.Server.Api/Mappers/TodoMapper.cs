﻿using MybitProject.Server.Api.Models.Todo;
using MybitProject.Shared.Dtos.Todo;
using Riok.Mapperly.Abstractions;

namespace MybitProject.Server.Api.Mappers;

/// <summary>
/// More info at Server/Mappers/README.md
/// </summary>
[Mapper]
public static partial class TodoMapper
{
    public static partial IQueryable<TodoItemDto> Project(this IQueryable<TodoItem> query);
    public static partial TodoItemDto Map(this TodoItem source);
    public static partial TodoItem Map(this TodoItemDto source);
    public static partial void Patch(this TodoItemDto source, TodoItem destination);
}
