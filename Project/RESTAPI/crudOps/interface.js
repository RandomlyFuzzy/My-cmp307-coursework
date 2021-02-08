const utils = require('./Utils')
const sql = require('mssql');
const Utils = require('./Utils');
async function TSQLInterface() {
    //private
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
            //to keep track of the request made without external access to the table
            try {
                //needs to be updated to a solitary function
                qry2 = qry.replace(/'/g, "\"");
                await this.GetResponseNoneQuery(null, null, `insert top(1) into Twang.AccessLog ( uid,Query,ipaddress) (select a.uid,'${qry2}' as Query,a.accessip from Twang.UserAccessStore as a where a.uid= ${UID} ) ORDER BY addtime DESC`, {})
            } catch (ex) {
                //their is a false positive error here :/ it still updates the DB
                //console.log(qry2);
                //console.log(ex);
                //console.trace();
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
                //console.log(ex);
            }
        }
        let resp = [];
        try {
            //console.log(1);
            //console.log("params", params);
            await ps.prepare(qry)
            //console.log(2);

            resp = await ps.execute(params)

            //console.log(3);
            await ps.unprepare(err => console.error)
            //console.log(4);
        } catch (ex) {
            //console.log(ex);
        }
        //console.log("response", resp.recordset)
        return resp.recordset;
    }

    this.GetResponseNoneQuery = async (UID, Transaction, qry, dict) => {
        if (UID != null) {
            //to keep track of the request made without external access to the table
            try {
                //needs to be updated to a solitary function
                qry2 = qry.replace(/'/g, "\"");
                await this.GetResponseNoneQuery(null, null, `insert top(1) into Twang.AccessLog ( uid,Query,ipaddress) (select a.uid,'${qry2}' as Query,a.accessip from Twang.UserAccessStore as a where a.uid= ${UID} ) ORDER BY addtime DESC`, {})
            } catch (ex) {
                //their is a false positive error here :/ it still updates the DB
                //console.log(qry2);
                console.error(ex);
                //console.trace();
            }
        }
        if (qry == "") {
            //console.trace();
        } else {
            //console.log(qry)
        }
        const ps = new sql.PreparedStatement()
        let params = {};
        for (const key in dict) {
            try {
                let typ = dict[key].type.type != undefined ? dict[key].type.type : dict[key].type;

                ps.input(key, typ)
                params[key] = Utils.Convert(dict[key].value, typ);

            } catch (ex) {
                //console.log(ex);
            }
        }
        let resp = [];
        try {
            //console.log(1);
            //console.log("params", params);
            await ps.prepare(qry)
            //console.log(2);

            resp = await ps.execute(params)

            //console.log(3);
            await ps.unprepare(err => console.error)
            //console.log(4);
        } catch (ex) {
            //console.log(ex);
        }



        //console.log("response", resp.rowsAffected)
        return resp.rowsAffected;
    }


    sql.on('error', async err => {
        //console.error(err);

        conn = await sql.connect(`mssql://${uname}:${pw}@${Serverip}/${dbname}`)
    })
    return this;
}


module.exports = TSQLInterface;
