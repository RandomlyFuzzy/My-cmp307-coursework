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


/*
// qpredicate the router with a check and bail out when needed
router.use(function (req, res, next) {
    if (req.method != "GET") return next('router')
    next()
})
// predicate the router with a check and bail out when needed
router.use(function (req, res, next) {
    if (!req.headers['x-auth']) return next('router')
    next()
})

// use the router and 401 anything falling through
app.use('/admin', router, function (req, res) {
    res.sendStatus(401)
})


router.get('/user/:id', function (req, res) {
    res.send('hello, user!')
})

*/


// router.param('user_id', function (req, res, next, id) {
//     res.send(id);
//     console.log('CALLED ONLY ONCE')
//     next()
// })

app.get("/test", (req, res) => {
    res.sendFile(__dirname + "/html/testing.html");
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



let pre = function (req, res, next) {
    console.log("request made from user", req.args[1]);
    next();
}

let get = async function (req, res, next) {
    // let val = await sqlinterface.GetResponse(null, null, null);
    req.args[3] = (req.query);
    //console.log(req.args);
    //console.log(req.query);
    let a;
    try {
        a = await Cache.FindOrQuery(...req.args);
    } catch (ex) {
        //todo post error code + message
        console.error(ex)
        res.sendStatus(Number.parseInt(ex) != null ? Number.parseInt(ex) : 500);
    }
    // console.log(a);
    res.send(a);

    next();
}

let post = async function (req, res, next) {
    // // just an example of maybe updating the user
    // req.user.name = req.params.name
    // // save user ... etc
    // req.res.data.push(req.user)
    // res.send("no");

    req.args[3] = (req.body);


    // console.log(req.args);
    let a;
    try {
        a = await Cache.Update(...req.args);
        //console.log(a);
        if (a[0] == 1) {
            res.sendStatus(200);
            // res.send(a);
        } else {
            res.sendStatus(404);
        }
    } catch (ex) {
        //todo post error code + message
        console.error(ex)
        res.sendStatus(Number.parseInt(ex.message) != NaN ? Number.parseInt(ex.message) : 500);
    }
    next();
}

let put = async function (req, res, next) {
    // console.log(req.query);
    // console.log(req.body);
    req.args[2] = {}
    for (const key in req.query) {
        req.args[2][key] = req.query[key];
    }
    for (const key in req.body) {
        req.args[2][key] = req.body[key];
    }
    //console.log("put", req.args);
    let a;
    try {
        a = await Cache.Insert(...req.args);
        // console.log("put", a);
        if (a[0] == 1) {
            res.sendStatus(200);
            // res.send(a);
        } else {
            res.sendStatus(404);
        }
    } catch (ex) {
        //todo post error code + message
        console.error(ex)
        await res.sendStatus(!isNaN(ex.message) ? Number.parseInt(ex.message) : 500);
    }
    next();
    // next(new Error('not implemented'))
}

let del = async function (req, res, next) {
    // res.send("yes");
    //console.log(req.args);
    let a;
    try {
        a = await Cache.Delete(...req.args)
        // console.log(a);
        if (a[0] == 1) {
            res.sendStatus(200);
            // res.send(a);
        } else {
            res.sendStatus(404);
        }
    } catch (ex) {
        //todo post error code + message
        console.error(ex)
        res.sendStatus(Number.parseInt(ex.message) != NaN ? Number.parseInt(ex.message) : 500);
    }
    next();
    // next(new Error('not implemented'))
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

//im pritty sure I got the put and post the wrong way around but i got too deep into development to change it

app.route('/DATA/:key/:table')
    .all(pre)
    .get(get)
    .put(put)
    .post((req, res, next) => {//can have 2 primary key present so this operation is invalid
        res.sendStatus(400)
        // res.send("Invalid request made")
    })
    .delete((req, res, next) => {//to make the operation as safe as possible
        res.sendStatus(400)
        // res.send("Invalid request made")
    })


app.route('/DATA/:key/:table/:pk')
    .all(pre)
    .get(get)
    .put((req, res, next) => {//should only be 1 primary key present to make things consistant for this
        res.sendStatus(400)
        // res.send("Invalid request made")
    })
    .post(post)
    .delete(del)



// var options = {
//     key: fs.readFileSync('./ssl/server.key'),
//     cert: fs.readFileSync('./ssl/server.crt'),
// };

http.createServer(app).listen(settings.port)
// https.createServer(options, app).listen(settings.port)
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
