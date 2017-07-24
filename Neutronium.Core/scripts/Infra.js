function Enum(type, intValue, name, displayName) {
    this.intValue = intValue;
    this.displayName = displayName;
    this.name = name;
    this.type = type;
    Object.defineProperty(this, '__readonly__', { value: true });
}

function Null_reference() {
}