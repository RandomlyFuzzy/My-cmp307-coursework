
function isDefault(value) {
    if (value == null) return true;
    if (value == undefined) return true;
    if (value == NaN) return true;

    switch (typeof (value)) {
        case "string":
            return value == "";
            break;
        case "number":
            return value == 0;
            break;
        default:
            return false;
    }
}



module.exports = {
    "SELECT": 0, "UPDATE": 1, "INSERT": 2, "DELETE": 3,
    isDefault: isDefault,
    Convert: (value, typ) => {
        switch (typ.declaration) {
            case "varchar":
                return "" + value;
                break;
            case "int":
                if (isDefault(+value)) {
                    value = 0;
                }
                return Number.parseInt(value);
                break;
            case "date":
                return new Date(value);
                break;
            case "datetime":
                return new Date(value);
                break;
            case "float":
                return Number.parseFloat(value);
                break;
            case "boolean":
                return value == true;
                break;
            default:
                return value
                break;
        }
    }
}