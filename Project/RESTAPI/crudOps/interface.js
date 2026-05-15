const sql = require('mssql');
const Utils = require('./Utils');
async function TSQLInterface() {
    //this is old - hardcoded credentials for university MSSQL server, needs env vars for production
    var uname = "mssql1902540";
    var pw = "Y9wsdUhjqQ";
    var dbname = "mssql1902540";
    var Serverip = "tolmount.abertay.ac.uk";
    var conn = await sql.connect(`mssql://${uname}:${pw}@${Serverip}/${dbname}`)

    //public
    this.startTransaction = () => {
        return new sql.Transaction(conn);

    }


    this.revertTransaction = (Transaction) => {


    }

    this.prepareStatement = async () => {

    }

    this.GetResponse = async (UID, Transaction, qry, dict) => {
        if (UID != null) {
            try {
                qry2 = qry.replace(/'/g, "\"");
                await this.GetResponseNoneQuery(null, null, `insert top(1) into Twang.AccessLog ( uid,Query,ipaddress) (select a.uid,'${qry2}' as Query,a.accessip from Twang.UserAccessStore as a where a.uid= ${UID} ) ORDER BY addtime DESC`, {})
            } catch (ex) {
            }
        }
        const ps = new sql.PreparedStatement()
        let params = {};
        for (const key in dict) {
            try {
                let typ = dict[key].type.type != undefined ? dict[key].type.type : dict[key].type;
                ps.input(key, typ)
                params[key] = Utils.Convert(dict[key].value, typ);
            } catch (ex) {
            }
        }
        let resp = [];
        try {
            await ps.prepare(qry)
            resp = await ps.execute(params)
            await ps.unprepare(err => console.error)
        } catch (ex) {
        }
        return resp.recordset;
    }

    this.GetResponseNoneQuery = async (UID, Transaction, qry, dict) => {
        if (UID != null) {
            try {
                qry2 = qry.replace(/'/g, "\"");
                await this.GetResponseNoneQuery(null, null, `insert top(1) into Twang.AccessLog ( uid,Query,ipaddress) (select a.uid,'${qry2}' as Query,a.accessip from Twang.UserAccessStore as a where a.uid= ${UID} ) ORDER BY addtime DESC`, {})
            } catch (ex) {
                console.error(ex);
            }
        }
        const ps = new sql.PreparedStatement()
        let params = {};
        for (const key in dict) {
            try {
                let typ = dict[key].type.type != undefined ? dict[key].type.type : dict[key].type;
                ps.input(key, typ)
                params[key] = Utils.Convert(dict[key].value, typ);
            } catch (ex) {
            }
        }
        let resp = [];
        try {
            await ps.prepare(qry)
            resp = await ps.execute(params)
            await ps.unprepare(err => console.error)
        } catch (ex) {
        }
        return resp.rowsAffected;
    }

    sql.on('error', async err => {
        conn = await sql.connect(`mssql://${uname}:${pw}@${Serverip}/${dbname}`)
    })
    return this;
}


module.exports = TSQLInterface;
