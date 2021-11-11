import "../App.css"
import {Tree} from './Tree.jsx'
import {Star} from './Star.jsx'
import { useCallback, useEffect, useState } from "react"
import { consumeSQS, getTop6 } from '../api.js'


const Scoreboard = () => {
  const [players, setPlayers] = useState([]);

  const setupListener = useCallback(() => {
    consumeSQS(res => {
      if (res) setPlayers(JSON.parse(res));
      setupListener();
    })
  }, []);

  useEffect(() => {
    getTop6(setPlayers);
    setupListener();
  }, [setupListener])
  
    return (
        <div className="Scoreboard">
          <Star 
            name={players[0] ? players[0].name : ""} 
            time={players[0] ? players[0].time : ""}
          />  
          <Tree players={players}/>
        </div>
    );
}

export default Scoreboard;