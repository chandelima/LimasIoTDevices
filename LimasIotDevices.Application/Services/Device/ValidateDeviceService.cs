using LimasIotDevices.Application.Constants;
using LimasIotDevices.Application.Services.HomeAssistant;
using LimasIotDevices.Domain.Interfaces.Gateways;
using LimasIotDevices.Domain.Models;
using LimasIotDevices.Infrastructure.Data;
using LimasIoTDevices.Facade.Dtos;
using LimasIoTDevices.Shared.Exceptions;
using LimasIoTDevices.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LimasIotDevices.Application.Services.Device;

internal class ValidateDeviceService
{
    private readonly TranslationModel _localizationModel;
    private readonly LimasIotDevicesDbContext _dbContext;
    private readonly GetExistentHomeAssistantDevicesService _getExistentHomeAssistantDevicesService;

    public ValidateDeviceService(
        TranslationModel localizationModel,
        LimasIotDevicesDbContext dbContext,
        GetExistentHomeAssistantDevicesService getExistentHomeAssistantDevicesService)
    {
        _localizationModel = localizationModel;
        _dbContext = dbContext;
        _getExistentHomeAssistantDevicesService = getExistentHomeAssistantDevicesService;
    }

    public async Task Execute(CreateUpdateDeviceRequest request, bool validateExistence = true)
    {
        ValidateFieldsAndThrow(request);

        var errorMessages = new List<string>();

        if (validateExistence)
        {
            var keyTrimmed = request.Key!.Trim();
    
            var sameDeviceKeyExists = _dbContext.Devices.Any(x => keyTrimmed == x.Key);
            if (sameDeviceKeyExists)
            {
                var msg = string.Format(_localizationModel.Device.Validation.KeyAlreadyRegistered, keyTrimmed);
                errorMessages.Add(msg);
            }

            var nameTrimmed = request.Name!.Trim();
            var sameDeviceNameExists = _dbContext.Devices.Any(x => nameTrimmed == x.Name);
            if (sameDeviceNameExists)
            {
                var msg = string.Format(_localizationModel.Device.Validation.NameAlreadyRegistered, nameTrimmed);
                errorMessages.Add(msg);
            }
        }

        var requestHaDevices = request.Attributes!.SelectMany(x => x.Entities!).ToList();
        var haDevices = await _getExistentHomeAssistantDevicesService.Execute(requestHaDevices);
        if (requestHaDevices.Count != haDevices.Count)
        {
            var inexistentDevices = requestHaDevices.Except(haDevices.Select(x => x.EntityId)).ToList();
            foreach (var inexistentDevice in inexistentDevices)
            {
                var msg = string.Format(_localizationModel.Device.Validation.HomeAssistantDeviceNotFound, inexistentDevice);
                errorMessages.Add(msg);
            }
        }

        if (errorMessages.Any())
        {
            throw new MessageErrorException(errorMessages);
        }
    }

    private void ValidateFieldsAndThrow(CreateUpdateDeviceRequest request)
    {
        var errorMessages = new List<string>();

        if (string.IsNullOrWhiteSpace(request?.Key))
        {
            errorMessages.Add(_localizationModel.Device.Validation.KeyRequired);
        }
        else if (request?.Key.Length > 255)
        {
            errorMessages.Add(_localizationModel.Device.Validation.KeyMaxLength);
        }

        if (string.IsNullOrWhiteSpace(request?.Name))
        {
            errorMessages.Add(_localizationModel.Device.Validation.NameRequired);
        }
        else if (request?.Name is { Length: > 255 })
        {
            errorMessages.Add(_localizationModel.Device.Validation.NameMaxLength);
        }

        if (request?.Description is { Length: > 500 })
        {
            errorMessages.Add(_localizationModel.Device.Validation.DescriptionMaxLength);
        }

        if (request?.Attributes is null or { Count: 0 })
        {
            errorMessages.Add(_localizationModel.Device.Validation.AttributeAtLeastOneRequired);
        }
        else
        {
            var containsStateAttribute = request.Attributes.Any(x => x.Key == ApplicationConstants.MAIN_KEY_NAME);
            if (!containsStateAttribute)
            {
                errorMessages.Add(string.Format(_localizationModel.Device.Validation.StateAttributeRequired, ApplicationConstants.MAIN_KEY_NAME));
            }

            var duplicatedAttributeKeys = request.Attributes.Select(x => x.Key)!.GetDuplicated();
            if (duplicatedAttributeKeys.Any())
            {
                var duplicated = string.Join(", ", duplicatedAttributeKeys);
                errorMessages.Add(string.Format(_localizationModel.DeviceAttribute.Validation.KeyDuplicated, duplicated));
            }

            var duplicatedNameKeys = request.Attributes.Select(x => x.Name)!.GetDuplicated();
            if (duplicatedNameKeys.Any())
            {
                var duplicated = string.Join(", ", duplicatedNameKeys);
                errorMessages.Add(string.Format(_localizationModel.DeviceAttribute.Validation.NameDuplicated, duplicated));
            }

            for (int a = 0; a < request.Attributes.Count; a++)
            {
                var attribute = request.Attributes[a];
                var attrId = string.IsNullOrWhiteSpace(attribute?.Key)
                    ? (a + 1).ToString()
                    : attribute?.Key;

                if (string.IsNullOrWhiteSpace(attribute?.Key))
                {
                    errorMessages.Add(string.Format(_localizationModel.DeviceAttribute.Validation.KeyRequired, attrId));
                }
                else if (attribute?.Key.Length > 255)
                {
                    errorMessages.Add(string.Format(_localizationModel.DeviceAttribute.Validation.KeyMaxLength, attrId));
                }

                if (string.IsNullOrWhiteSpace(attribute?.Name))
                {
                    errorMessages.Add(string.Format(_localizationModel.DeviceAttribute.Validation.NameRequired, attrId));
                }
                else if (attribute?.Name is { Length: > 255 })
                {
                    errorMessages.Add(string.Format(_localizationModel.DeviceAttribute.Validation.NameMaxLength, attrId));
                }

                if (attribute?.Description is { Length: > 500 })
                {
                    errorMessages.Add(string.Format(_localizationModel.DeviceAttribute.Validation.DescriptionMaxLength, attrId));
                }

                if (attribute?.Entities is null or { Length: 0 })
                {
                    errorMessages.Add(string.Format(_localizationModel.DeviceAttribute.Validation.EntityRequired, attrId));
                }
                else
                {
                    if (attribute.Entities.Any(string.IsNullOrWhiteSpace))
                    {
                        errorMessages.Add(string.Format(_localizationModel.DeviceAttribute.Validation.EntityNameRequired, attrId));
                    }

                    if (attribute.Entities.Any(e => e is { Length: > 255 }))
                    {
                        errorMessages.Add(string.Format(_localizationModel.DeviceAttribute.Validation.EntityNameMaxLength, attrId));
                    }
                }
            }
        }

        if (!errorMessages.Any())
        {
            return;
        }

        throw new MessageErrorException(errorMessages);
    }
}
 