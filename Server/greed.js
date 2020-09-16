// TODO: update _etag so that Orleans Grain does not flush old state
// OR do updates via https://dotnet.github.io/orleans/Documentation/clusters_and_clients/powershell_client.html
var newname = "7586943210"; // IvanBo // 7586943210
db.getCollection('GrainsPlayerGrain').find({ "_doc.Nickname": newname})
var bratishka = db.getCollection('GrainsPlayerGrain').find({ "_doc.Nickname": newname})[0]._doc.Id;
db.getCollection('GrainsArmyGrain').find({"_doc.PlayerId" : bratishka});
var players = db.getCollection('GrainsPlayerGrain');
//players.findOneAndUpdate({ "_doc.Nickname": newname}, {  $unset:{"_doc.Gold" : 1 }}, null);
//players.findOneAndUpdate({ "_doc.Nickname": newname}, {  $set:{"_doc.Gold" : NumberInt(42000) }}, null);
//players.findOneAndUpdate({ "_doc.Nickname": newname}, {  $set:{"_doc.Level" : NumberInt(11) }}, null);