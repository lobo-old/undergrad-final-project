import { Meteor } from 'meteor/meteor';
import { Template } from 'meteor/templating';
 
import { ReadRequests } from '../api/datasets.js';//observar que o caminho relativo é diferente

import './TempReadRequest.html';

Template.TempReadRequest.helpers({
  readName(serverN){
  	console.log(serverN);
  	return serverN;
  	//return ReadRequests.find({serverName: serverN});
    //return ReadRequests.find({},{ sort: { createdAt: -1 }, limit:10  });
  },
  displayName(serverN, requestT, keyword) {
  	const test = ReadRequests.find({},{ sort: { createdAt: -1 }, limit:10  }).fetch();
  	console.log(test);
  	console.log(test[0]);
  	console.log(test[0].serverName);
    var prefix = keyword.hash.title ? keyword.hash.title + " " : "";
    return prefix + serverN + " " + requestT;
  },
  	
});
 
Template.TempReadRequest.events({
  'click .delete-ReadRequest'() {
    //ReadRequests.remove(this._id);
    Meteor.call('readRequests.removeReadRequests', this._id);
  },
});