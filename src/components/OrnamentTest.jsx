import "../App.css"
import "./OrnamentTest.css"
import Proptypes from 'prop-types'

const OrnamentTest = ({player, color, place}) => {

    return(
        <div className={`ornament-container ${color}`}>
            {player && 
            <div className={`ornament ${color}`}>
                <div>{place}</div>
                <div>{player.name}</div>
                <div>{player.time}</div>
                <div className="ornament-shadow"></div>
        </div> }
       </div>
    )
}

export default OrnamentTest;
OrnamentTest.propTypes = {
    color: Proptypes.oneOf(['red', 'pink', 'yellow', 'orange', 'blue', 'white']).isRequired,
    place: Proptypes.oneOf(['2nd', '3rd', '4th', '5th', '6th']).isRequired
}