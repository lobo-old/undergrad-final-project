import { Meteor } from 'meteor/meteor';
import { Template } from 'meteor/templating';

import { WriteRequests } from '../imports/api/datasets.js';
import { ReadRequests } from '../imports/api/datasets.js';
import { DevicesAvailable } from '../imports/api/datasets.js';
import { ServersOPC } from '../imports/api/datasets.js';

//Import de todos os teplates utilizados
import './main.html';
//import '../imports/ui/TempReadRequest.html';  Já FOI importado no respectivo javaScript
//import '../imports/ui/TempWriteRequest.html';

import '../imports/ui/TempReadRequest.js'
import '../imports/ui/TempWriteRequest.js'

//subscribe do client
Meteor.subscribe("allWriteRequests");
Meteor.subscribe("allReadRequests");
Meteor.subscribe("allDevicesAvailable");
Meteor.subscribe("allServersOPC");
Meteor.subscribe("allPeriodicRequests");

Template.body.helpers({
  serversOPC(){                 //retorna todos os documentos que representam os servidores OPC registrados no banco de dados
    return ServersOPC.find({});
  },
  readRequestsSubmited(){       //Retorna os documentos das 10 últimas requests de leitura 
    return ReadRequests.find({},{ sort: { createdAt: -1 }, limit:10  });
  },
  writeRequestsSubmited(){      //Retorna os documentos das 10 últimas requests de escrita
    return WriteRequests.find({},{ sort: { createdAt: -1 }, limit:10 });
  },
  devicesAvailableInServer(serverN){//Retorna os documentos que representam os dispositivos de um servidor OPC 
    return DevicesAvailable.find({serverName: serverN});
  },
  devicesAvailable(){
    return DevicesAvailable.find({});
  },
  periodicStatus(comm) {
    //console.log(comm);
    if(comm == 'started'){
      return true;
    }else{
      if (comm == 'stopped') {}
      return false; 
    }
    return false;
  },
  idParserTabControl(serverId) {
    //console.log(serverId);
    return serverId._str;
  },
  displayValue(serverN, devID) {
    const test = ReadRequests.find({serverName: serverN, requestT: devID, checked: true},{ sort: { createdAt: -1 }, limit:1   }).fetch();
    //ESTÁ SENDO ORDENADO PELA DATA DE CRIAÇÃO E NAO A DE CHECAGEM, VERIFICAR
    //console.log(test);
    if(test.length != 0){//se o array não está vazio
      //console.log(test[0]);
      if(test[0].checked){//redundante porque a request já faz isso
        //console.log(test[0].responseValue);//só existe .responseValue se já foi checada
        return test[0].responseValue;
      }else{
        return "";  
      }
    }else{
      return "";
    }
    //return prefix + serverN + " " + devID;
  },
  displayTStamp(serverN, devID) {
    const test = ReadRequests.find({serverName: serverN, requestT: devID, checked: true},{ sort: { createdAt: -1 }, limit:1  }).fetch();
    //console.log(test);
    if(test.length != 0){//se o array não está vazio
      //console.log(test[0]);
      if(test[0].checked){//redundante porque a request já faz isso
        //console.log(test[0].checkedAt.getUTCDate());
        //console.log(test[0].checkedAt);//só existe .checkedAt se já foi checada
        return test[0].checkedAt;
      }else{
        return "";  
      }
    }else{
      return "";
    }
    //return prefix + serverN + " " + devID;
  },

});

Template.body.events({
  'click .device-ReadRequest'(event) {//clique no botão de leitura do dispositivo
    // Chamando método para inserir um novo pedido de leitura no banco de dados
    Meteor.call('readRequests.insertReadRequests',this.serverName,this.deviceID)
  },
  'submit .deviceNew-writeRequest'(event) {//formulário é submetido
    event.preventDefault();
    const target = event.target; // Pegando valor do elemento
    const valueRequest = target.devWriteRequestServer.value;
    // Chamando método para inserir um novo pedido de escrita no banco de dados
    Meteor.call('writeRequests.insertWriteRequests',this.serverName,this.deviceID,valueRequest)
    // Clear form
    target.devWriteRequestServer.value = '';
  },
  'click .start-periodic-inServer'(event) {
    //editar document deste server, campo checked false e 
    //ServersOPC.update(this._id, {$set: { checked: false, command: 'started'}});
    //CHAMAR METEOR CALL
    Meteor.call('serversOPC.demandStart',this._id)
    //console.log(this.serverName);
    //console.log('start click');
  },
  'click .stop-periodic-inServer'(event) {
    Meteor.call('serversOPC.demandStop',this._id)
    //console.log(this.serverName);
    //console.log(this.connected);
  },
  'submit .new-readRequest'(event) {
    // Prevent default browser form submit
    event.preventDefault();
 
    // Get value from form element
    const target = event.target;
    //console.log(event.target);
    //console.log(target);
    const textServer = target.readRequestServer.value;
    const textRequest = target.readRequestText.value; //ESTÁ ACESSANDO PELA TAG name que aqui é requestText
    
    // Insert a task into the collection
    Meteor.call('readRequests.insertReadRequests',textServer,textRequest)
 
    // Clear form
    //target.readRequestText.value = '';
  },
  'submit .new-writeRequest'(event) {
    // Prevent default browser form submit
    event.preventDefault();
 
    // Get value from form element
    const target = event.target;
    //console.log(target);
    const textServer = target.writeRequestServer.value;
    const textRequest = target.writeRequestText.value; //ESTÁ ACESSANDO PELA TAG name que aqui é requestText
    const valueRequest = target.writeRequestValue.value;
    //console.log(textRequest);
    //console.log(valueRequest);
    // Insert a task into the collection
    Meteor.call('writeRequests.insertWriteRequests',textServer,textRequest,valueRequest)
 
    // Clear form
    //target.writeRequestText.value = '';
    target.writeRequestValue.value = '';
  },
});