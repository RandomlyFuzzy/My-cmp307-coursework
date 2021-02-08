const sql = require('mssql');
let state = require("./Utils");
let Query = require("./Query");



// four classes are in this file as they are all apart of a recursive/cyclical structures
{

    //helper functions for determining if the input params are 
    //      a) of the valid type 
    //      b) if primary key not a duplicate 
    //      c) if FK has something to link to
    //      d) make sure that if its required its included in the insert staement 
    let types = {
        "string": ValidateString,
        "number": ValidateNumber,
        "int": ValidateInt,
        "datetime": ValidateDateTime,
        "date": ValidateDateTime,
        "FK": ValidateFK,
    }

    function ValidateString(maxSize) {
        return (check) => {
            return typeof (check) == typeof ("") && check.length <= maxSize;
        }
    }
    function ValidateInt() {
        return (check) => {
            try {
                let ret = check.match(/(-|)[0-9]*/g)
                return ret[0] == check;//works for all numbers big or small
            } catch (ex) {
                return false;
            }
        }
    }
    function ValidateNumber() {
        return (check) => {
            try {
                return +check;
            } catch (ex) {
                let ret = check.match(/(-|)[0-9\.]*/g)
                return ret[0] == check;
            }
        }
    }
    function ValidateDateTime() {
        return (check) => {
            try {
                new Date(check);
                return true;
            } catch {
                return false;
            }
        }
    }
    function ValidateFK(table, collumn, type, args) {
        return (check) => {
            let checking = {};
            checking[collumn] = check;
            let ret = null;
            ret = Cache.FindOrQuery(null, table, "", checking).then(a => a != null);
            while (ret == null);
            if (!ret) {
                ////console.log("cannot find FK for ", collumn, " with value ", check, " on table ", table);
            }

            return types[typeof (type)](...args)(check) && ret;
        }
    }

    function collumn(type, args = [], maintype = null, isPK = false, isFk = false, required = true) {
        this.dbtype = sql.VarChar(0)
        // ////console.log(maintype ?? type.constructor.name);

        //major datatypes defined
        sqltype = maintype == null ? type.constructor.name : maintype
        switch (sqltype) {
            case "String":
                this.dbtype = sql.VarChar(args[0])
                break;
            case "int":
                this.dbtype = sql.Int
                break;
            case "date":
                this.dbtype = sql.Date
                break;
            case "datetime":
                this.dbtype = sql.DateTime
                break;
            case "float":
                this.dbtype = sql.Float
                break;
            case "boolean":
                this.dbtype = sql.Bit
                break;
            default:
                break;
        }
        this.isRequired = () => required
        this.check = (table, check, mode) => {
            let ret = true;
            if (isFk && mode == state.INSERT) {
                if (args.length > 3) {
                    args[3] = args.splice(3, args.length);
                } else if (args.length < 3) {
                    ////console.log("invalid amount of args passed ");
                } else {
                    args[3] = [];
                }
                ret = ret && types["FK"](...args)(check)
            } else {
                let DataType = maintype == null ? type.constructor.name : maintype
                DataType = DataType.toLowerCase();
                // console.log(DataType)
                ret = ret && types[DataType](...args)(check)
            }
            if (isPK && mode == state.INSERT) {

                ret = ret && (Cache.FindOrQuery(null, table, check).then(a => a == null || a == undefined));
            }
            return ret;
        }
    }

}
class tableSchemas {

    //yes this not a typical json schema this is just an easy to decode schema 
    //and can check inputs as if they were going to be passed for sql
    static tables = {
        "Twang.Users": {
            "PK": "id",
            "id": new collumn((0), [], "int", true),
            "uname": new collumn((""), [255]),
            "pkey": new collumn((""), [255])
        },
        "Twang.Room": {
            "PK": "id",
            "id": new collumn((0), [], "int", true),
            "rname": new collumn((""), [255]),
            "capacity": new collumn((0), [], "int")
        },
        "Twang.Booking": {
            "PK": "id",
            "id": new collumn((0), [], "int", true),
            "rid": new collumn((0), ["Twang.Room", "id", 0], "int", false, true),
            "title": new collumn((""), [255]),
            "starttime": new collumn((""), [], "datetime"),
            "duration": new collumn((0), [], "int"),
            "bookingTime": new collumn((""), [], "datetime", false, false, false),
            "bookingDate": new collumn((""), [], "date", false, false, false),
            "oid": new collumn((0), ["Twang.Users", "id", 0], "int", false, true),
        },
        //keeping track of the ip address
        "Twang.UserAccessStore": {
            "PK": "id",
            "id": new collumn((0), [], "int", true),
            "uid": new collumn((0), ["Twang.Users", "id", 0], "int", false, true),
            "accessip": new collumn((""), [255], null, false, false, false),
            "addtime": new collumn((""), [], "datetime", false, false, false),
            "Logofftime": new collumn((""), [], "datetime", false, false, false),
        },
        "Twang.UserBooking": {
            "PK": "id",
            "id": new collumn((0), [], "int", true),
            "uid": new collumn((0), ["Twang.Users", "id", 0], "int", false, true),
            "bid": new collumn((0), ["Twang.Booking", "id", 0], "int", false, true),
        },
        "Twang.UserPerms": {
            "PK": "id",
            "id": new collumn((0), [], "int", true),
            "uid": new collumn((0), ["Twang.Users", "id", 0], "int", false, true),
            "tabl": new collumn((""), [30], null, false, false, false),
            "attribute": new collumn((""), [1], null, false, false, false),
        }
    }

    constructor() { }

    static GetPK(tablename) {
        try {
            return tableSchemas.tables[tablename]["PK"];
        } catch (ex) {
            ////console.error(ex);
        }
    }

    /**
     * should not need to 
     */
    validateAgainst(tablename, values, mode) {
        ////console.log(this.constructor.name);
        if (tableSchemas.tables[tablename.toString()] == undefined) {
            console.log("invalid table passed")
            console.trace();
            return false;
        }

        //is of valid type
        for (const key in values) {

            if (state.isDefault(values[key])) {
                continue;
            }
            try {

                if ((tableSchemas.tables[tablename][key] == undefined || (!tableSchemas.tables[tablename][key].check(tablename, values[key], mode)))) {
                    console.log("invalid params passed", values[key], key, tablename)
                    console.trace();
                    (!tableSchemas.tables[tablename][key].check(tablename, values[key], mode))
                    return false;
                }
            }
            catch (ex) {
                console.log(ex);
                return false;
            }
        }

        //is required
        if (mode == state.INSERT) {
            let ret = true;

            for (const key in tableSchemas.tables[tablename]) {
                if (state.isDefault(values[key])) {
                    continue;
                }
                try {
                    if (typeof (tableSchemas.tables[tablename][key]) != typeof ("") && tableSchemas.tables[tablename][key].isRequired()) {
                        if (values[key] == undefined) {
                            console.log("missing params passed", key, tablename)
                            return false;
                        }
                    }
                } catch (ex) {
                    console.log(ex);
                    return false;
                }
            }
        }
        return true;
    }

    GenerateColumnTypesTable(tablename, values) {
        if (tableSchemas.tables[tablename] == undefined) {
            ////console.log("invalid table passed")
            return null;
        }
        let ret = {};
        if (values.length != {}) {
            for (const key in values) {
                if (!state.isDefault(values[key]) && (tableSchemas.tables[tablename][key] != typeof (""))) {
                    ret[key] = ({ "value": values[key], "type": tableSchemas.tables[tablename][key].dbtype })
                }
            }
            if (ret[tableSchemas.tables[tablename]["PK"]] == undefined) {
                ret[tableSchemas.tables[tablename]["PK"]] = ({ "value": null, "type": tableSchemas.tables[tablename][tableSchemas.tables[tablename]["PK"]].dbtype })
            }
        }


        return ret;
    }
}
let schemas = new tableSchemas();
class Validate {

    constructor() {
    }
    //is correct table
    //has corrent values
    //PK
    isValidQuery(table, mode, values, PK = null) {
        // ////console.log(arguments);
        switch (mode) {
            case state.SELECT:
                return schemas.validateAgainst(table, values)
                break;
            case state.INSERT:
                if (PK != null) {
                    values[schem.GetPK(table)] = PK;
                } else if (values[tableSchemas.GetPK(table)] != undefined) {
                } else {
                    ////console.log("No pk passed for INSERT command")
                    ////console.log(arguments);
                    return false;
                }
                return schemas.validateAgainst(table, values, state.INSERT)
                break;
            case state.UPDATE:
                return schemas.validateAgainst(table, values)
                break;
            case state.DELETE:
                if (PK == null) {
                    ////console.error("No pk passed for Delete command")
                    return false;
                }
                let obj = {};
                obj[tableSchemas.GetPK(table)] = PK;
                return schemas.validateAgainst(table, obj)
                break;
            default:
                ////console.log("invalid state passed")
                return false;
                break;
        }

    }

    async GetAsJson(UID, Tablename, mode, checkingagainst = {}, PK = null) {
        // ////console.log(arguments);
        let que;
        let ret;
        switch (mode) {
            case state.SELECT:
                if (PK == null) {
                    que = new Query(UID, state.SELECT, Tablename, schemas.GenerateColumnTypesTable(Tablename, checkingagainst))
                } else {
                    try {
                        que = new Query(UID, state.SELECT, Tablename, schemas.GenerateColumnTypesTable(Tablename, checkingagainst), tableSchemas.GetPK(Tablename), PK)
                    } catch (ex) {
                        console.error(ex);
                    }
                }
                try {
                    ret = await que.GetResult();
                    // ////console.log("json", ret)
                } catch (ex) {
                    ////console.error(ex);
                }
                break;
            case state.INSERT:
                if (PK == null) {
                    que = new Query(UID, state.INSERT, Tablename, schemas.GenerateColumnTypesTable(Tablename, checkingagainst))
                } else {
                    que = new Query(UID, state.INSERT, Tablename, schemas.GenerateColumnTypesTable(Tablename, checkingagainst), tableSchemas.GetPK(Tablename), PK)
                }
                ret = await que.GetResult();
                break;
            case state.UPDATE:
                //will need to always have the pk of an entry
                que = new Query(UID, state.UPDATE, Tablename, schemas.GenerateColumnTypesTable(Tablename, checkingagainst), tableSchemas.GetPK(Tablename), PK)
                ////console.log(que);
                ret = await que.GetResult();
                break;
            case state.DELETE:
                let pkname = tableSchemas.GetPK(Tablename);
                let objs = {};
                objs[pkname] = PK;
                que = new Query(UID, state.DELETE, Tablename, schemas.GenerateColumnTypesTable(Tablename, objs), pkname, PK)
                ////console.log(que);
                ret = await que.GetResult();
                break;
                break;
            default:
                return false;
                break;
        }
        // ////console.log(ret);
        return ret;
    }
}
let validater = new Validate();
function Authenticate() {

    /**
     * this is going to be in the form of ["token":userPK,...]
     */
    let KEYS = {}
    /**
     * this is going to be in the form of [userPK:"token",...]
     */
    let KEYSinvers = {}



    this.checkCredentials = async (token) => {
        if (KEYS[token] == undefined) {
            return false;
        }
        return KEYS[token]
    }

    this.GenerateKey = (UID) => {
        let keylen = 32;
        let Charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
        let ret = "";
        for (let i = 0; i < keylen; i++) {
            ret += Charset[Math.floor(Math.random() * Charset.length)];
        }
        if (KEYS[ret] != undefined) {
            return GenerateKey(UID)//incase a key is already present
        }
        return { "token": ret };
    }



    this.Login = async (Uname, UID, ipaddress) => {
        //check user is already logged in
        if (KEYSinvers[UID] != undefined) {
            await this.LogOut(KEYSinvers[UID]);//this needs to be done incase someone changes computers
        }
        ////console.log(arguments);
        let check = await Cache.FindOrQuery(null, "Twang.Users", UID, { uname: Uname });

        // ////console.log(check);
        if (check == undefined || check.length != 1) {
            return false;
        }

        await Cache.Insert(null, "Twang.UserAccessStore", { "id": "" + Math.floor(Math.random() * (1 << 15)), "uid": UID, "accessip": ipaddress, "Logofftime": new Date(1753, 0, 1, 0, 0, 0, 0).toISOString() })


        let token = this.GenerateKey(UID)
        KEYS[token.token] = UID;
        KEYSinvers[UID] = token.token;
        return token;
    }

    this.LogOut = async (token) => {

        if (KEYSinvers[KEYS[token]] == undefined) {
            return false;
        }

        let UID = KEYS[token];
        let resp = await Cache.FindOrQuery(UID, "Twang.UserAccessStore", "", { "uid": UID });//only people who have acces to this server code directly or the MSSQL server can update this table
        let Recent = resp.sort(function (a, b) { return new Date(a.addtime) > new Date(b.addtime) ? -1 : 1; })[0]//update most recent entry
        await Cache.Update(null, "Twang.UserAccessStore", Recent.id, { "Logofftime": new Date().toISOString() })//SQL updates logoff time;
        KEYSinvers[KEYS[token]] = undefined;
        KEYS[token] = undefined;
        return true;
    }

    this.CheckPerms = async (UID, table, operation) => {
        //intial state
        if (UID == undefined) {
            return false;
        }
        //me specified
        if (UID == null) {
            return true;
        }

        //because I do not plan on implementing the permission table into this solution in tableschema class
        //it will be its own entity excisting when needed, also enables instant permission changes 
        let que = new Query(UID, state.SELECT, "Twang.UserPerms", schemas.GenerateColumnTypesTable("Twang.UserPerms", { "uid": UID, "tabl": table }))


        let ret = await que.GetResult();


        //all users if no specific perms set;
        if (ret.length == 0) {
            switch (operation) {
                case state.SELECT:
                    switch (table) {
                        case "Twang.UserPerms"://needed for prepared statments
                            return false;
                        default:
                            return true;
                    }
                    break;
                case state.INSERT:
                    switch (table) {
                        case "Twang.Booking":
                        case "Twang.UserBooking":
                            return true;
                        default:
                            return false;
                    }
                    break;
                case state.UPDATE:
                    return false;
                case state.DELETE:
                    return false;
                default:
                    return false;
            }
        }
        //if a perm is overriden 
        ret = ((ret[0].attribute.charCodeAt(0) >> operation) & 1) == 1;

        ////console.log(ret);
        return ret;
    }
}
let authenticate = new Authenticate();
class Cache {
    //this will be in the form of
    /**
     * Data = {
     *  "tablename":[PK:{Entry},...],
     *  "tablename":[PK:{Entry},...]
     * }
     *  where pk can be of any practically type
     */

    //would make this #Data but will generate errors in my current docker(cant find any other consistant images :( )
    static Data = {}

    constructor() { }
    //would make this #Find but will generate errors in my current docker(cant find any other consistant images :( )
    static Find(UID, Tablename, pkvalue = "", checkingagainst = {}) {
        if (validater.isValidQuery(Tablename, state.SELECT, checkingagainst, pkvalue)) {
            if (Cache.Data[Tablename] == undefined) {
                return null;
            }
            // ////console.log(arguments)
            if (pkvalue != "") {
                // ////console.log(Cache.Data[Tablename][pkvalue]);
                return [Cache.Data[Tablename][pkvalue]] == [] ? null : [Cache.Data[Tablename][pkvalue]]
            }
            let ret = [];
            for (const key in Cache.Data[Tablename]) {
                if (checkingagainst != {}) {
                    let add = false;
                    for (const its in checkingagainst) {
                        if (Cache.Data[Tablename][key][its] == checkingagainst[its]) {
                            add = true;
                            break;
                        }
                    }
                    if (add) {
                        ret.push(Cache.Data[Tablename][key])
                    }
                } else {
                    ret.push(Cache.Data[Tablename][key])
                }
            }
            return ret.length == 0 ? null : ret;
        } else {
            throw new Error("400");
        }
        return null;
    }

    //would make this #query but will generate errors in my current docker(cant find any other consistant images :( )
    static async Query(UID, Tablename, pkvalue = "", checkingagainst = {}) {
        // ////console.log("args = " + JSON.stringify(arguments));
        if (validater.isValidQuery(Tablename, state.SELECT, checkingagainst, pkvalue)) {
            return validater.GetAsJson(UID, Tablename, state.SELECT, checkingagainst, pkvalue)
        } else {
            throw new Error("400");
        }
        return null;
    }

    static async FindOrQuery(UID, Tablename, pkvalue = "", checkingagainst = {}) {
        if (UID != null) {
            if (!(await authenticate.CheckPerms(UID, Tablename, state.SELECT))) {
                throw new Error(401);
            }
        }
        ////console.log(arguments, Tablename);
        let ret = this.Find(UID, Tablename, pkvalue, checkingagainst);
        if (ret == null) {
            ret = await this.Query(UID, Tablename, pkvalue, checkingagainst);
            // ////console.log("query");
            //////console.log(ret);
        }
        return ret;
    }

    static async Insert(UID, Tablename, collumns = {}) {
        if (UID != null) {
            if (!(await authenticate.CheckPerms(UID, Tablename, state.INSERT))) {
                throw new Error(401);
            }
        }
        if (validater.isValidQuery(Tablename, state.INSERT, collumns)) {
            let ret = 0;
            ret = await validater.GetAsJson(UID, Tablename, state.INSERT, collumns)
            let add = await this.FindOrQuery(UID, Tablename, "", collumns);
            return ret;
        } else {
            throw new Error(400);
        }
    }

    static async Update(UID, Tablename, pkvalue = "", checkingagainst = {}) {
        if (UID != null) {
            if (!(await authenticate.CheckPerms(UID, Tablename, state.UPDATE))) {
                throw new Error("401");
            }
        }
        let ret = 0;
        if (validater.isValidQuery(Tablename, state.UPDATE, checkingagainst, pkvalue)) {
            ret = await validater.GetAsJson(UID, Tablename, state.UPDATE, checkingagainst, pkvalue)
            for (const key in checkingagainst) {
                if (this.Data[Tablename] == undefined) {
                    this.Data[Tablename] = {};
                }
                if (this.Data[Tablename][pkvalue] == undefined) {
                    this.Data[Tablename][pkvalue] = {};
                }
                this.Data[Tablename][pkvalue][key] = checkingagainst[key];
            }
            return [ret];
        } else {
            throw new Error("400");
        }
    }

    static async Delete(UID, Tablename, pkvalue = "") {
        if (UID != null) {
            if (!(await authenticate.CheckPerms(UID, Tablename, state.DELETE))) {
                throw new Error(401);
            }
        }
        let ret = 0;
        if (validater.isValidQuery(Tablename, state.DELETE, {}, pkvalue)) {
            ret = await validater.GetAsJson(UID, Tablename, state.DELETE, {}, pkvalue);
            ////console.log("del", ret);
            if (this.Data[Tablename] != undefined && this.Data[Tablename][pkvalue] != undefined) {
                this.Data[Tablename][pkvalue] = undefined;
            }
            return ret;
        } else {
            throw new Error("400");
        }
    }
}




module.exports = { "Cache": Cache, "Validate": Validate, "Schemas": tableSchemas, "Authenticate": Authenticate };
