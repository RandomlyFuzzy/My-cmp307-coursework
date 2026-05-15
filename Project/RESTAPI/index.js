const express = require("express");
const fs = require("fs");
const app = express()
const router = express.Router()
const Interface = require("./crudOps/interface");
var https = require('https')
var http = require('http')
var settings = require(process.argv[2] == null ? "./settings/Settings.json" : process.argv[2]);
var atob = require('atob');
var { Cache } = require("./crudOps/Cache");
let Auth = new (require("./crudOps/Cache").Authenticate)();


var sqlinterface;

Interface().then(async inter => (sqlinterface = (await inter)));//should fix




app.use(express.urlencoded({ extended: true }))
app.use(function (err, req, res, next) {
    console.error(err.stack)
    res.status(500).send('Something broke!')
})
app.use(function (req, res, next) {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Methods', 'GET, PUT, POST, DELETE, OPTIONS');
    res.header('Access-Control-Allow-Headers', 'Content-Type, Authorization, Content-Length, X-Requested-With');

    //intercepts OPTIONS method
    if ('OPTIONS' === req.method) {
        //respond with 200
        res.sendStatus(200);
    }
    else {
        //move on
        next();
    }
});


app.get("/test", (req, res) => {
    res.sendStatus(200);
})



app.route('/loginadmin')
    .get(async (req, res) => {
        console.log(req.headers);
        if (req.get('authorization') == undefined || atob(req.get('authorization').substr(6)).trim() != "202E8BFDD5ADCBF2A6642B024FFD3733084596C0D59941D9AAC1169DFD43FF159C61B49C1FAE40F928AC6037590425C849DB32AD4DF3A12E4367AA5756BEA0FD") {//sha512 hash of SupersecretPassword
            res.sendStatus(401);
            return;
        }

        if (req.query == {}) {
            res.sendStatus(401);
            return;
        }
        if (req.query.uname == undefined || typeof (req.query.uname) != typeof ("")) {
            console.log("a");
            res.sendStatus(401);
            return;
        }
        let ret
        let ipadd = req.socket.remoteAddress.substr(7)
        try {
            ret = await Auth.Login(req.query.uname, req.query.id, ipadd == "" ? "localhost" : ipadd);
        } catch (ex) {
            //todo post error code + message
            console.error(ex)
            res.sendStatus(Number.parseInt(ex) != NaN ? Number.parseInt(ex) : 500);
        }

        if (!ret) {
            res.sendStatus(401);
            return;
        }
        //console.log(ret);
        res.json([ret]);
        return;
    });


app.route('/login')
    .get(async (req, res) => {

        let ipadd = req.socket.remoteAddress.substr(7)

        if (req.query == {}) {
            res.sendStatus(401);
            return;
        }
        if (
            req.query.uname == undefined || typeof (req.query.uname) != typeof ("") ||
            req.query.id == -1 || req.query.uname == "Admin User"
        ) {
            res.sendStatus(401);
            return;
        }
        let ret
        try {
            ret = await Auth.Login(req.query.uname, req.query.id, ipadd == "" ? "localhost" : ipadd);
        } catch (ex) {
            //todo post error code + message
            console.error(ex)
            res.sendStatus(Number.parseInt(ex) != NaN ? Number.parseInt(ex) : 500);
        }

        if (!ret) {
            res.sendStatus(401);
            return;
        }
        //console.log(ret);
        res.json([ret]);
        return;
    });
app.route('/logout')
    .get(async (req, res) => {
        res.send(await Auth.LogOut(req.query.token));
    });



let get = async function (req, res, next) {
    req.args[3] = (req.query);
    let a;
    try {
        a = await Cache.FindOrQuery(...req.args);
    } catch (ex) {
        console.error(ex)
        res.sendStatus(Number.parseInt(ex) != null ? Number.parseInt(ex) : 500);
    }
    res.send(a);
    next();
}

let post = async function (req, res, next) {
    req.args[3] = (req.body);
    let a;
    try {
        a = await Cache.Update(...req.args);
        if (a[0] == 1) {
            res.sendStatus(200);
        } else {
            res.sendStatus(404);
        }
    } catch (ex) {
        console.error(ex)
        res.sendStatus(Number.parseInt(ex.message) != NaN ? Number.parseInt(ex.message) : 500);
    }
    next();
}

let put = async function (req, res, next) {
    req.args[2] = {}
    for (const key in req.query) {
        req.args[2][key] = req.query[key];
    }
    for (const key in req.body) {
        req.args[2][key] = req.body[key];
    }
    let a;
    try {
        a = await Cache.Insert(...req.args);
        if (a[0] == 1) {
            res.sendStatus(200);
        } else {
            res.sendStatus(404);
        }
    } catch (ex) {
        console.error(ex)
        await res.sendStatus(!isNaN(ex.message) ? Number.parseInt(ex.message) : 500);
    }
    next();
}

let del = async function (req, res, next) {
    let a;
    try {
        a = await Cache.Delete(...req.args)
        if (a[0] == 1) {
            res.sendStatus(200);
        } else {
            res.sendStatus(404);
        }
    } catch (ex) {
        console.error(ex)
        res.sendStatus(Number.parseInt(ex.message) != NaN ? Number.parseInt(ex.message) : 500);
    }
    next();
}


app.options("/*", function (req, res, next) {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Methods', 'GET,PUT,POST,DELETE,OPTIONS');
    res.header('Access-Control-Allow-Headers', 'Content-Type, Authorization, Content-Length, X-Requested-With');
    res.send(200);
});


//should do it through x-Auth-key 
app.param('key', async function (req, res, next, key) {
    /**
     * Check if valid creds
     * else return ERROR
     */

    req.args = [];
    let ret = await Auth.checkCredentials(key)
    if (!ret) {
        res.sendStatus(401)
        return;
    }
    req.args.push(ret)
    console.log('KEY', key)
    next()
})

app.param('table', function (req, res, next, table) {
    /**
     * Check if valid table(not case sensitive)
     * else return ERROR
     */
    req.args.push(table);
    console.log("Table = ", table);
    next()
})

app.param('pk', function (req, res, next, id) {
    /**
     * Set locally for the request
     */
    req.args[2] = (id);
    console.log("pk = ", id);
    next()
})

app.route('/DATA/:key/:table')
    .get(get)
    .put(put)
    .post((req, res, next) => {
        res.sendStatus(400)
    })
    .delete((req, res, next) => {
        res.sendStatus(400)
    })


app.route('/DATA/:key/:table/:pk')
    .get(get)
    .put((req, res, next) => {
        res.sendStatus(400)
    })
    .post(post)
    .delete(del)



http.createServer(app).listen(settings.port)
async function removeMeetings() {
    //

    if (sqlinterface == undefined) {
        sqlinterface = await (Interface());
    }
    //deletes all FKs and the main entree
    //delete from Twang.UserBooking where bid in (Select id as bid from Twang.Booking WHERE starttime >  DATEADD(month, -6, getdate()));
    let a = await sqlinterface.GetResponseNoneQuery(null, null, "delete from Twang.UserBooking where bid in (Select id as bid from Twang.Booking WHERE starttime >  DATEADD(month, -6, getdate()));", {});
    a = await sqlinterface.GetResponseNoneQuery(null, null, "delete from Twang.Booking WHERE starttime < DATEADD(month, -6, getdate())", {});
    if (a != 0) {
        console.log(a, " Bookings were deleted")
    }

    a = await sqlinterface.GetResponseNoneQuery(null, null, "delete from Twang.UserAccessStore WHERE addtime <  DATEADD(month, -6, getdate())", {});
    if (a != 0) {
        console.log(a, " User access logs were deleted")
    }
}
function removemeetingTimeout() {
    setTimeout(removeMeetings, 1000 * 60 * 24)
}

removeMeetings();

//midnight tonight
let temp = new Date();
temp.setHours(24, 0, 0, 0)
temp.setTime(temp.getTime() - (new Date()).getTime())

setTimeout(removemeetingTimeout, temp.getTime())
