rs.initiate( {
    _id : "PocketShooter",
    members: [
      { _id: 0, host: "10.32.27.208:27018" },
      { _id: 1, host: "10.32.27.194:27018" },
      { _id: 2, host: "10.32.27.195:27018" }
    ]
});

rs.conf();
rs.status();

// db.getSiblingDB("admin").createUser(
//   {
//     user: "root",
//     pwd: "<ask>",
//     roles: [ 
//       { role: "root", db: "admin" }
//     ]
//   }
// );

// db.getSiblingDB("PocketShooter").createUser(
//   {
//     user: "PocketShooter_backup",
//     pwd: "<ask>",
//     roles: [ 
//       { role: "backup", db: "admin" }
//     ]
//   }
// );


// PocketShooter_readWrite
db.getSiblingDB("PocketShooter").dropUser("PocketShooter_readWrite");
db.getSiblingDB("PocketShooter").createUser(
  {
    user: "PocketShooter_readWrite",
    pwd: "POAc213wewd2aS",
    roles: [ 
      { role: "readWrite", db: "PocketShooter" },
      { role: "readWrite", db: "admin" }
    ]
  }
);
db.getSiblingDB("PocketShooter").getUser("PocketShooter_readWrite");

db.getSiblingDB("admin").dropUser("PocketShooter_readWrite");
db.getSiblingDB("admin").createUser(
  {
    user: "PocketShooter_readWrite",
    pwd: "POAc213wewd2aS",
    roles: [ 
      { role: "readWrite", db: "PocketShooter" },
      { role: "readWrite", db: "admin" }
    ]
  }
);
db.getSiblingDB("admin").getUser("PocketShooter_readWrite");
//////


////// PocketShooter_read
db.getSiblingDB("PocketShooter").createUser(
  {
    user: "PocketShooter_read",
    pwd: "ewqQL0(7711!==!00",
    roles: [ 
      { role: "read", db: "PocketShooter" }
    ]
  }
);
//////

////// clusterMonitor

db.getSiblingDB("admin").createUser(
  {
    user: "clusterMonitor",
    pwd: ")@!J#K@!#8932kdsfj8",
    roles: [ 
      { role: "clusterMonitor", db: "admin" }
    ]
  }
);

////// 

db.getSiblingDB("admin").getUsers();
db.getSiblingDB("PocketShooter").getUsers();
