import { Meteor } from 'meteor/meteor';
import { Template } from 'meteor/templating';
 
//import { WriteRequests } from '../api/datasets.js';//observar que o caminho relativo é diferente

import './TempWriteRequest.html';
 
Template.TempWriteRequest.events({
  'click .delete-WriteRequest'() {
    Meteor.call('writeRequests.removeWriteRequests', this._id);
  },
});