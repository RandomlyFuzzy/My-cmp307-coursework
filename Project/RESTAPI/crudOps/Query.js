
var sqlInterfaceModule = require("./interface");
let Utils = require("./Utils")


var sqlinterface;

sqlInterfaceModule().then(async inter => (sqlinterface = (await inter)));


function Query(UID, state, tablename, value, pkn = null, pkval = null) {
    this.table = tablename;
    this.pkname = pkn || "";
    this.pkvalue = pkval || "";
    this.values = value || {};

    if (this.pkname != "" && Utils.isDefault(this.pkvalue) && this.values[this.pkname] == undefined) {
        this.values[this.pkname].value = this.pkvalue;
    }

    if (this.values != {} && this.pkname != "" && !state.UPDATE && this.values[this.pkname] != undefined) {
        this.values[this.pkname].value = this.pkvalue;
    }


    this.operationstate = state;
    this.requestedUsers = UID;

    this.GetResult = () => {
        let query = "";
        switch (this.operationstate) {
            case Utils.SELECT:
                query = `SELECT * from ${this.table}`
                for (const key in this.values) {
                    if (Utils.isDefault(this.values[key].value)) {
                        delete this.values[key];
                    }
                }
                if (this.values != {}) {
                    query += " where ";
                    let cnt = 0;
                    let added = false;
                    for (const key in this.values) {
                        if (this.values[key].value == null) {

                        }
                        if (!Utils.isDefault(this.values[key]) && this.values[key].value != null) {
                            added = true;
                            query += `${key} = @${key} and `;
                        } else {
                            delete this.values[key];
                            continue;
                        }
                    }
                    if (added) {
                        query = query.substr(0, query.length - 4);
                    } else {
                        query = query.substr(0, query.length - 7);
                    }
                }


                if (sqlinterface == null) {
                    return sqlInterfaceModule()
                        .then(async inter => (sqlinterface = (await inter)))
                        .then(async inter => await inter.GetResponse(this.requestedUsers, null, query, this.values));
                } else {
                    return sqlinterface.GetResponse(this.requestedUsers, null, query, this.values).then(a => a);
                }
                break;
            case Utils.INSERT:
                query = `INSERT into ${this.table} (`
                query2 = ` values (`
                let added = false;
                for (const key in this.values) {
                    if (Utils.isDefault(this.values[key].value)) {
                        delete this.values[key];
                    }
                }
                if (this.values != {}) {
                    for (const key in this.values) {
                        if (!Utils.isDefault(this.values[key])) {
                            added = true;
                            query += `${key},`;
                            query2 += `@${key},`;
                        }
                    }
                }
                query = query.substr(0, query.length - 1);
                query += ")"
                query2 = query2.substr(0, query2.length - 1);
                query2 += ")"
                query += query2;
                if (!added) {
                    console.error(" insert statement is baddly requested " + JSON.stringify(this));
                }
                break;
            case Utils.UPDATE:
                query = `UPDATE ${this.table} SET `
                for (const key in this.values) {
                    query += `${key} = @${key} ,`
                }
                query = query.substr(0, query.length - 1);
                query += `where ${this.pkname} = @PK`
                this.values["PK"] = this.values[this.pkname];
                this.values["PK"].value = this.pkvalue;
                break;
            case Utils.DELETE:
                query = `DELETE FROM ${this.table} WHERE ${this.pkname} = @${this.pkname} `
                this.values["PK"] = this.values[this.pkname];
                this.values["PK"].value = this.pkvalue;
                break;
            default:
                break;
        }

        for (const key in this.values) {
            if (Utils.isDefault(this.values[key].value)) {
                delete this.values[key];
            }
        }
        if (sqlinterface == null) {
            return sqlInterfaceModule()
                .then(async inter => (sqlinterface = (await inter)))
                .then(async inter => await inter.GetResponseNoneQuery(this.requestedUsers, null, query, this.values));
        } else {
            return sqlinterface.GetResponseNoneQuery(this.requestedUsers, null, query, this.values).then(a => a);
        }
    }
}

module.exports = Query;