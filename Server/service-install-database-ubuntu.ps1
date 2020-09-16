# https://docs.mongodb.com/manual/tutorial/install-mongodb-on-ubuntu/
# https://docs.mongodb.com/manual/tutorial/deploy-replica-set/
# TODO: read all from topology.yml
param([string]$computer, [string]$node, [string]$environment,[string]$user="root")

ssh -o UserKnownHostsFile=.deployment/production/.ssh/known_hosts ${user}@${computer} sudo service mongod stop
scp -o UserKnownHostsFile=.deployment/production/.ssh/known_hosts .deployment/production/$node/mongod.conf $user@${computer}:/etc/
ssh -o UserKnownHostsFile=.deployment/production/.ssh/known_hosts ${user}@${computer} sudo service mongod start





