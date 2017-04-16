//@auth 
//@req(pathFrom, nodeGroup)

var mountFrom = "${nodes.build.first.id}";
var envName = "${settings.targetEnv}".split(".")[0];
var mountTo = nodeGroup;

//TODO:move autoDeployFolder deterination logic to jem
var autoDeployFolder, type;
var nodes = jelastic.env.control.GetEnvInfo(envName, session).nodes;
for (var i = 0, l = nodes.length; i < l; i++) {
       if (nodes[i].nodeGroup == nodeGroup) {
              type = nodes[i].nodeType;
              if (type.indexOf("tomcat") > -1) {
                     autoDeployFolder = "/opt/tomcat/webapps";
              } else if (type == "glassfish4") {
                     autoDeployFolder="/opt/repo/versions/"+nodes[i].version+"/glassfish/domains/domain1/autodeploy";
              } if (type == "glassfish3") {
                     autoDeployFolder="/opt/"+type+"/glassfish/domains/domain1/autodeploy";
              } if (type == "wildfly10") {
                     autoDeployFolder="/opt/repo/versions/"+nodes[i].version+"/standalone/deployments";
              }
              break;
       }
}

if (!autoDeployFolder) {
   return {result: 99, type: "error", message: "autodeploy folder is not defined for nodeType=" + type}
}
//---

var resp = jelastic.env.file.RemoveMountPointByGroup(envName, session, mountTo, autoDeployFolder);
if (resp.result != 0) return resp;

resp = jelastic.env.file.AddMountPointByGroup(envName, session, mountTo, autoDeployFolder, 'nfs', null, pathFrom, mountFrom, 'auto-deploy-folder', false); 
return resp;
