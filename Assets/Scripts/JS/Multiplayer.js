import SimpleJSON;

function ExecuteAction(args) {
    var json = JSON.Parse(args);

    var action = json["action"];
    var ec2_url = "http://ec2-13-59-209-203.us-east-2.compute.amazonaws.com:4300/vpk/api/v1/kaiju/" + json["id"] + "/";
    var beanstalk_url = "http://vpkapi.dcj8tjfhw6.us-east-2.elasticbeanstalk.com/api/v1/";

    var form = new WWWForm();
    if(action == 1) {
        form.AddField( "method", "send");
        form.AddField( "id", json["id"].ToString().Replace('"',''));
        form.AddField( "kaiju_name", json["kaiju_name"].ToString().Replace('"',''));
        form.AddField( "energy", json["energy"].ToString().Replace('"',''));
        form.AddField( "strength", json["strength"].ToString().Replace('"',''));
        form.AddField( "defense", json["defense"].ToString().Replace('"',''));
    }
    else if (action == 2) {
         form.AddField( "method", "fetch");
         form.AddField( "id", json["id"].ToString().Replace('"',''));  
         beanstalk_url = beanstalk_url + "kaiju/";
              
    }  
    else if (action == 3) {
        beanstalk_url = beanstalk_url + "kaiju/" + json["id"].ToString().Replace('"','') + "/";   
    }
    else if (action == 4) {
    	beanstalk_url = beanstalk_url + "leaderboards/" + json["id"].ToString().Replace('"','') + "/" + json["leaderboard_tag"].ToString().Replace('"','') + "/";   

    }

    var www = new WWW( beanstalk_url, form );

    if(action == 3 || action == 4) www = new WWW( beanstalk_url );   
     
    // wait for request to complete
    yield www;
 
    // and check for errors
    if (www.error == null)
    {
        // request completed!
        var response = JSON.Parse(www.text);
        if(action != 4) {
        	var kaiju = response["kaiju"];
        	var kaiju_energy = response["kaiju"]["energy"].ToString().Replace('"','').Replace('\\','');
        	var battles_json = response["kaiju"]["battles"];
        	var battle_results = new Array();
        	var battle_ids = new Array();
        	for(var j = 0; j < battles_json.Count; j++)
        	{
        	    battle_results.push(battles_json[j]["winnerID"].ToString().Replace('"',''));
        	    battle_ids.push(battles_json[j]["id"].ToString().Replace('"',''));
        	}
        }
        else {
        	var leaderboard_kaiju = response["kaiju"];
        	var leaderboards = response["leaderboards"];

        }
        if(action == 1) this.SendMessage("ReceiveSentConfirmation", kaiju);
        else if(action == 2)this.SendMessage("ReceiveFetchConfirmation", [battle_results, battle_ids, kaiju_energy]);
        else if(action == 3)this.SendMessage("ReceiveGetConfirmation", [battle_results, battle_ids, kaiju_energy]);
        else if(action == 4)this.SendMessage("ReceiveGetLeaderboardsConfirmation", [leaderboard_kaiju, leaderboards]);
    } else {
        // something wrong!
        Debug.Log("WWW Error: "+ www.error);
        if(action == 1) this.SendMessage("ReceiveSentError", "sent error");
        else if(action == 2) this.SendMessage("ReceiveFetchError", "fetch error");
        else if(action == 3) this.SendMessage("ReceiveGetError", "get error");
        else if(action == 4) this.SendMessage("ReceiveGetLeaderboardsError", "get leaderboards error");
    }

}
