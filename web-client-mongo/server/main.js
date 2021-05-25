import { Meteor } from 'meteor/meteor';

import { WriteRequests } from '../imports/api/datasets.js';
import { ReadRequests } from '../imports/api/datasets.js';
import { DevicesAvailable } from '../imports/api/datasets.js';
import { ServersOPC } from '../imports/api/datasets.js';
import { PeriodicRequests } from '../imports/api/datasets.js';

if (Meteor.isServer) {
     //publicando para os clientes os dados: EVITAR PUBLICAR TODOS OS DADOS
    Meteor.publish("allWriteRequests", function(){
        return WriteRequests.find({});
    });
    
    Meteor.publish("allReadRequests", function(){
        return ReadRequests.find({});
    });

    Meteor.publish("allDevicesAvailable", function(){
        return DevicesAvailable.find({});
    });

    Meteor.publish("allServersOPC", function(){
        return ServersOPC.find({});
    });

    Meteor.publish("allPeriodicRequests", function(){
        return PeriodicRequests.find({});
    });
}

Meteor.startup(() => {
  // code to run on server at startup
});


