import { Meteor } from 'meteor/meteor';
import { Mongo } from 'meteor/mongo';
import { check } from 'meteor/check';

export const WriteRequests = new Mongo.Collection('writeRequests');
export const ReadRequests = new Mongo.Collection('readRequests');
export const DevicesAvailable = new Mongo.Collection('devicesAvailable');
export const ServersOPC = new Mongo.Collection('serversOPC');
export const PeriodicRequests = new Mongo.Collection('periodicReadRequests');

//métodos que serão utilizados para verificar a origem das requests e se os dados informados são consistentes
Meteor.methods({
  'serversOPC.demandStart'(taskId) {
    //console.log(taskId);
    //check(taskId, String);
    ServersOPC.update(taskId, {$set: { checked: false, command: 'started'}});
  },
  'serversOPC.demandStop'(taskId) {
    //console.log(taskId);
    //check(taskId, String);
    ServersOPC.update(taskId, {$set: { checked: false, command: 'stopped'}});
  },
  'readRequests.insertReadRequests'(serverText,text) {//verificar se o nome inicial deve ser datasets
    check(serverText, String);
    check(text, String);
 
    // Make sure the user is logged in before inserting a task
    //if (! Meteor.userId()) {
    //  throw new Meteor.Error('not-authorized');
    //}
 
    ReadRequests.insert({
      requestT: text,
      serverName: serverText,
      createdAt: new Date(), // current time
      //checkedAt: new Date(),//criar do tipo date porém com valor vazio
      responseTime: 0,//tempo em segundos desde a criação da request até ser resolvida pelo servidor(diferenças de DateTime)
      checked: false,
    });
  },
  'readRequests.removeReadRequests'(taskId) {
    //check(taskId, String);
 
    ReadRequests.remove(taskId);
  },
  'writeRequests.insertWriteRequests'(serverText,text,value) {//verificar se o nome inicial deve ser datasets
    check(serverText, String);
    check(text, String);
    check(value, String);// CONSEGUIR VERIFICAR SE O NÚMERO É TIPO: 5.6666
    // Make sure the user is logged in before inserting a task
    //if (! Meteor.userId()) {
    //  throw new Meteor.Error('not-authorized');
    //}
 
    WriteRequests.insert({
      requestT: text,
      serverName: serverText,
      createdAt: new Date(), // current time
      //checkedAt: new Date(),//criar do tipo date porém com valor vazio
      responseTime: 0,//tempo em segundos desde a criação da request até ser resolvida pelo servidor(diferenças de DateTime)
      requestValue: value,
      checked: false,
    });
  },
  'writeRequests.removeWriteRequests'(taskId) {
    //check(taskId, String);
 
    WriteRequests.remove(taskId);
  },
});



