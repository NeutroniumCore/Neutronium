function Enum(type, intValue, name, displayName) {
    this.intValue = intValue;
    this.displayName = displayName;
    this.name = name;
    this.type = type;
    Object.defineProperty(this, '{{NeutroniumConstants.ReadOnlyFlag}}', { value: {{ReadOnly}} });
}

function Null_reference() {
}