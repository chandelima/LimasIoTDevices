namespace LimasIotDevices.Domain.Models;

public class TranslationModel
{
    public DeviceTranslation Device { get; set; } = new();
    public DeviceAttributeTranslation DeviceAttribute { get; set; } = new();
}

public class DeviceTranslation
{
    public DeviceValidation Validation { get; set; } = new();
}

public class DeviceValidation
{
    public string AttributeAtLeastOneRequired { get; set; } = string.Empty;
    public string StateAttributeRequired { get; set; } = string.Empty;
    public string DescriptionMaxLength { get; set; } = string.Empty;
    public string KeyAlreadyRegistered { get; set; } = string.Empty;
    public string KeyRequired { get; set; } = string.Empty;
    public string KeyMaxLength { get; set; } = string.Empty;
    public string NameAlreadyRegistered { get; set; } = string.Empty;
    public string NameRequired { get; set; } = string.Empty;
    public string NameMaxLength { get; set; } = string.Empty;

    // Message used when a Home Assistant entity referenced by the request does not exist
    public string HomeAssistantDeviceNotFound { get; set; } = "Device \"{0}\" does not exist in HomeAssistant.";
}

public class DeviceAttributeTranslation
{
    public DeviceAttributeValidation Validation { get; set; } = new();
}

public class DeviceAttributeValidation
{
    public string DescriptionMaxLength { get; set; } = string.Empty;
    public string EntityAtLeastOneRequired { get; set; } = string.Empty;
    public string EntityRequired { get; set; } = string.Empty;
    public string EntityNameRequired { get; set; } = string.Empty;
    public string EntityNameMaxLength { get; set; } = string.Empty;
    public string KeyDuplicated { get; set; } = string.Empty;
    public string KeyMaxLength { get; set; } = string.Empty;
    public string KeyRequired { get; set; } = string.Empty;
    public string NameDuplicated { get; set; } = string.Empty;
    public string NameMaxLength { get; set; } = string.Empty;
    public string NameRequired { get; set; } = string.Empty;
}
