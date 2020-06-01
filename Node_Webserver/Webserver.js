var express = require('express');
var app = express();
var body_parser = require('body-parser');
const sqlite3 = require('sqlite3').verbose();

app.use (body_parser.json ()); 
app.use (body_parser.urlencoded ({extended : true}));

app.listen(3000,()=>{
    console.log("연결됨");
})

let db = new sqlite3.Database('data.db',sqlite3.OPEN_READWRITE,(err)=>{
    if(err){
       return console.error(err.message);
    }
    console.log('Connected to the database');
});

app.get('/GetWord',(req,res)=>{
    console.log('GetWord');
    db.all('select word from QuizWord order by random() limit 10;',(err,row)=>{
        if(err){
            res.send(err.message);
        }
         res.send(row);
    });
    
});

app.post('/login',(req,res)=>{
    console.log('login');
    var id = req.body.id;
    var password = req.body.password;
    var param = [id,password];

    selectAccountFunc(req,res,param);
   
 });
    
app.post('/makeAccount',(req,res)=>{
    console.log('makeAccount'); 
    var id = req.body.id;
    var password = req.body.password;
    var param = [id,password];

    db.serialize(()=>{
        db.run('insert into Account(id,password) values(?,?)',param,(err,row)=>{
            if(err){
                res.send(err.message);
                return;
            }
            selectAccountFunc(req,res,param);

        });
    });
        
        

 });

 app.post('/idChk',(req,res)=>{
    console.log('idChk'); 
    var id = req.body.id;
    var param = [id];

    db.all('select id from Account where id = ? ;',param,(err,row)=>{
        if(err){
            res.send(err.message);
            return;
        }
    
        if(row.length < 1){
            res.send(false);
        }else{
            res.send(true);
        }

 });
});

app.post('/updateProfile',(req,res)=>{
    console.log('updateProfile');
    var id = req.body.id;
    var profileImg = req.body.profileImg;

    var param = [profileImg,id];
    db.run('update Account set profileImg = ? where id = ? ',param,(err)=>{
        if(err){
            console.log(err.message);
            res.send(err.message);
            return;
        }
        console.log(`Row(s) updated: ${this.changes}`);
    });
})

app.post('/updateWinLose',(req,res)=>{
    console.log('updateWinLose');
    var id = req.body.id;
    var win = req.body.win;
    var lose = req.body.lose;

    var param = [win,lose,id];

    db.run('update Account set win = ?,lose = ? where id = ? ',param,(err)=>{
        if(err){
            console.log(err.message);
            res.send(err.message);
            return;
        }
        console.log(`Row(s) updated: ${this.changes}`);
    });
})

 var selectAccountFunc = function(req,res,param)
 {
    db.all('select id,win,lose,profileImg from Account where id = ? and password = ? ;',param,(err,row)=>{
        if(err){
            res.send(err.message);
            return;
        }

        if(row.length < 1){
            res.send(false);
        }else{
            res.send(row);
        }

    });
}