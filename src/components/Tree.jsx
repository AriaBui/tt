import "../App.css"
import OrnamentTest from "./OrnamentTest";
import gingerbread from '../assets/gingerbread.png';
import snowman from '../assets/snowman.png';
import candycane from '../assets/candycane.png';



export const Tree = ({players}) => {

    return (
    <div className="tree">
        <svg 
            width={909}
            height={820}
            viewBox="7 0 1009 1320"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
        >
            <g filter="url(#prefix__filter0_d_16:13)">
                <path
                d="M320.49 314.664L512.993 1 697.26 314.664l-74.119-19.574 147.723 230.477-106.545-35.721 202.797 236.838-149.267-27.892 214.635 246.625-171.914-16.149L1003 1171H6l220.297-257.88-171.914 32.297 245.518-246.625-146.178 27.892L356.52 489.846l-104.487 35.721L403.359 295.09l-82.869 19.574z"
                fill="#197E41"
                stroke="#197E41"
                />
            </g>
            <path d="M415 1171h177.71v148.11H415V1171z" fill="#AC6625" />

            {/* {players[1] && <>
                <ellipse cx={579} cy={224} rx={70} ry={70} fill="#E7B6EF" />
                <text fill="#000000" x="58%" y="15%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{"2nd"}</text>
                <text fill="#000000" x="58%" y="18%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{players[1].name}</text>
                <text fill="#000000" x="58%" y="20%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{players[1].time}</text>
            </>}

            {players[2] && <>
                <ellipse cx={350.195} cy={622.027} rx={70} ry={70} fill="#fd5" />
                <text fill="#000000" x="34.7%" y="45%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{"3rd"}</text>
                <text fill="#000000" x="34.7%" y="48%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{players[2].name}</text>
                <text fill="#000000" x="34.7%" y="50%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{players[2].time}</text>
            </>}

            {players[3] && <>
                <ellipse cx={713.195} cy={803.027} rx={70}  ry={70} fill="#DC9832" />
                <text fill="#000000" x="71%" y="59%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{"4th"}</text>
                <text fill="#000000" x="71%" y="62%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{players[3].name}</text>
                <text fill="#000000" x="71%" y="64%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{players[3].time}</text>
            </>}

            {players[4] && <>
                <ellipse cx={239.195} cy={1059.03} rx={70} ry={70} fill="#123456" />
                <text fill="#000000" x="24%" y="78%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{"5th"}</text>
                <text fill="#000000" x="24%" y="81%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{players[4].name}</text>
                <text fill="#000000" x="24%" y="83%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{players[4].time}</text>
            </>}
            
            {players[5] && <>
                <ellipse cx={816.195} cy={1095.03} rx={70} ry={70} fill="#DC3D32" />
            </>} */}
            <defs>
                <filter
        id="prefix__filter0_d_16:13"
        x={0.915}
        y={0.029}
        width={1007.29}
        height={1179.47}
        filterUnits="userSpaceOnUse"
        colorInterpolationFilters="sRGB"
    >
        <feFlood floodOpacity={0} result="BackgroundImageFix" />
        <feColorMatrix
        in="SourceAlpha"
        values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0"
        result="hardAlpha"
        />
        <feOffset dy={4} />
        <feGaussianBlur stdDeviation={2} />
        <feComposite in2="hardAlpha" operator="out" />
        <feColorMatrix values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.25 0" />
        <feBlend in2="BackgroundImageFix" result="effect1_dropShadow_16:13" />
        <feBlend
        in="SourceGraphic"
        in2="effect1_dropShadow_16:13"
        result="shape"
        />
    </filter>
            </defs>
        </svg>
        <OrnamentTest place="2nd" player={players[1]} color={'pink'}/>
        <OrnamentTest place="3rd" player={players[2]} color={'red'}/>
        <OrnamentTest place="4th" player={players[3]} color={'yellow'}/>
        <OrnamentTest place="5th" player={players[4]} color={'blue'}/>
        <OrnamentTest place="6th" player={players[4]} color={'white'}/>
        <div className="decoration">
            <img src={gingerbread} className="gingerbread"/>
            <img src={snowman} className="snowman"/>
            <img src={candycane} className="candycane1"/>
            <img src={candycane} className="candycane2"/>
            
        </div>
    </div>
    );

}

